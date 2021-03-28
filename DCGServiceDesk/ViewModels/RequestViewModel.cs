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
                interfaceContainer);
            NotEscalated.RequestViewModel = this;
            Escalation = new EscalationViewModel() { AssigmentGroups = singleRequest.Groups };
            CurrentMode = NotEscalated;
        }
        public async Task InitializeNEVMComboBoxes() =>
            await NotEscalated.SetComboBoxModels();
    }
    public class NotEscalatedViewModel:ViewModelBase
    {
        public NotEscalatedViewModel(TaskRequest t, DbInterfaceContainer dbInterfaces)
        {
            CurrentState = t.History.ActiveStatus.State;
            CurrentImpact = t.Impact;
            CurrentSubcategory = t.Category;
            CurrentUrgency = t.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
        }
        public NotEscalatedViewModel(Incident i, DbInterfaceContainer dbInterfaces)
        {
            CurrentState = i.History.ActiveStatus.State;
            CurrentImpact = i.Impact;
            CurrentSubcategory = i.Category;
            CurrentUrgency = i.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
        }
        public NotEscalatedViewModel(ServiceRequest c, DbInterfaceContainer dbInterfaces)
        {
            CurrentState = c.History.ActiveStatus.State;
            CurrentImpact = c.Impact;
            CurrentSubcategory = c.Category;
            CurrentUrgency = c.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
        }
        public ICommand FindUserCommand { get; }
        private readonly IRequestQueue _requestQueue;
        public RequestViewModel RequestViewModel { get; set; }
        public List<State> States { get; private set; }
        public List<Urgency> Urgencies { get; private set; }
        public List<Categorization> Categorizations { get; private set; }
        public List<Impact> Impacts { get; private set; }
        public List<Priority> Priorities { get; private set; }
        public List<CloserDue> CloserDues { get; private set; }

        private State _state;
        private Urgency _urgency;
        private Categorization _subcategory;
        private Impact _impact;
        private Priority _priority;
        private int _currentLevel;
        private AccountInfo _contact;
        private AccountInfo _recipient;
        private string _cUsername;
        private string _rUsername;

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
        public string CUsername
        {
            get { return _cUsername; }
            set
            {
                _cUsername = value;
                OnPropertyChanged("CUsername");
            }
        }
        public string RUsername
        {
            get { return _rUsername; }
            set
            {
                _rUsername = value;
                OnPropertyChanged("RUsername");
            }
        }
        public AccountInfo Contact
        {
            get { return _contact; }
            set
            {
                if (value != null)
                {
                    _contact = value;
                    OnPropertyChanged("Contact");
                }
            }
        }
        public AccountInfo Recipient
        {
            get { return _recipient; }
            set
            {
                if (value != null)
                {
                    _recipient = value;
                    OnPropertyChanged("Recipient");
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
            CUsername = RequestViewModel.WorkspaceInfo[0].CommunicationInfo.RequestedPerson;
            RUsername = RequestViewModel.WorkspaceInfo[0].CommunicationInfo.ContactPerson;
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

    public class AccountInfo
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Localization { get; set; }
        public string Position { get; set; }
        public string SuperiorName { get; set; }
        public string SuperiorSurname { get; set; }
        public string SuperiorUsername { get; set; }
        public string SuperiorEmail { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
    }
}
