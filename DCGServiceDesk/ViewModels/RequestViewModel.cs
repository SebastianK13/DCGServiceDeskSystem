using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class RequestViewModel:Tab
    {
        public RequestViewModel(SingleRequestInfo singleRequest, 
            DbInterfaceContainer interfaceContainer, HomeViewModel hVM) 
            : base(interfaceContainer, hVM)
        {
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = singleRequest.Request;
            wi.CommunicationInfo = singleRequest.Info;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = singleRequest.Label;
            NotEscalated = new NotEscalatedViewModel(RequestService.ConvertRequest(singleRequest.Request),
                interfaceContainer.RequestQueue);
            NotEscalated.RequestViewModel = this;
            Escalation = new EscalationViewModel() { AssigmentGroups = singleRequest.Groups };
            CurrentMode = NotEscalated;
        }
        public async Task InitializeNEVMComboBoxes() =>
            await NotEscalated.SetComboBoxModels();
    }
    public class NotEscalatedViewModel:ViewModelBase
    {
        public NotEscalatedViewModel(TaskRequest t, IRequestQueue requestQueue)
        {
            CurrentState = t.History.ActiveStatus.State;
            CurrentImpact = t.Impact;
            CurrentSubcategory = t.Category;
            CurrentUrgency = t.Urgency;
            _requestQueue = requestQueue;
        }
        public NotEscalatedViewModel(Incident i, IRequestQueue requestQueue)
        {
            CurrentState = i.History.ActiveStatus.State;
            _requestQueue = requestQueue;
        }
        public NotEscalatedViewModel(ServiceRequest c, IRequestQueue requestQueue)
        {
            CurrentState = c.History.ActiveStatus.State;
            _requestQueue = requestQueue;
        }
        private readonly IRequestQueue _requestQueue;
        public RequestViewModel RequestViewModel { get; set; }
        public List<State> States { get; private set; }
        public List<Urgency> Urgencies { get; private set; }
        public List<Categorization> Categorizations { get; private set; }
        public List<Impact> Impacts { get; private set; }
        public List<Priority> Priorities { get; private set; }
        public List<CloserDue> CloserDues { get; private set; }

        //public List<CloserDue> CloserDues { get; set; }
        private State _state;
        private Urgency _urgency;
        private Categorization _subcategory;
        private Impact _impact;
        private Priority _priority;
        private int _currentLevel;
        public State CurrentState
        {
            get { return _state; }
            set
            {
                if (value != null)
                {
                    _state = value;
                    OnPropertyChanged("CurrentState");
                }
            }
        } 
        public Categorization CurrentSubcategory
        {
            get { return _subcategory; }
            set
            {
                if (value != null)
                {
                    _subcategory = value;
                    OnPropertyChanged("CurrentSubcategory");
                }
            }
        }
        public Urgency CurrentUrgency
        {
            get { return _urgency; }
            set
            {
                if (value != null)
                {
                    _urgency = value;
                    SetPriority();
                    OnPropertyChanged("CurrentUrgency");
                }
            }
        }
        public Impact CurrentImpact
        {
            get { return _impact; }
            set
            {
                if (value != null)
                {
                    _impact = value;
                    SetPriority();
                    OnPropertyChanged("CurrentImpact");
                }
            }
        }
        public Priority CurrentPriority
        {
            get{ return _priority; }
            private set
            {
                if (value != null)
                {
                    _priority = value;
                    OnPropertyChanged("CurrentPriority");
                }
            }
        }
        public async Task SetComboBoxModels()
        {
            States = await _requestQueue.GetAllStates();
            Categorizations = await _requestQueue.GetSubcategories();
            Urgencies = await _requestQueue.GetUrgencies();
            Impacts = await _requestQueue.GetImpacts();
            Priorities = await _requestQueue.GetPriority();
            CloserDues = await _requestQueue.GetCloserDues();
            SetPriority();
        }
        private void SetPriority() 
        {
            if(CurrentUrgency!=null && CurrentImpact!=null && Priorities!=null)
            {
                _currentLevel = RequestService.GetPriorityLvl(CurrentUrgency.level, CurrentImpact.level);
                CurrentPriority = Priorities
                    .Where(l => l.level == _currentLevel)
                    .FirstOrDefault();
            }
        }
    }
    public class EscalationViewModel
    {
        public List<AssigmentGroup> AssigmentGroups { get; set; }
        public RequestViewModel RequestViewModel { get; set; }
    }
}
