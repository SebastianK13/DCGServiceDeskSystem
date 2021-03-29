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
using System.Windows.Media;

namespace DCGServiceDesk.ViewModels
{
    public class RequestViewModel : Tab
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
        public async Task InitializeNEVMModel()
        {
            await NotEscalated.Initialize();
            NotEscalated.SetInitialColors();
        }
    }
    public class NotEscalatedViewModel : ViewModelBase
    {
        public NotEscalatedViewModel(TaskRequest t, DbInterfaceContainer dbInterfaces)
        {
            CurrentState = t.History.ActiveStatus.State;
            CurrentImpact = t.Impact;
            CurrentSubcategory = t.Category;
            CurrentUrgency = t.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
            FindUserEventArea = false;
            CloseRequestCommand = new CloseRequestCommand(dbInterfaces, this);
            Topic = t.Topic;
            Description = t.Description;
            Solution = t.History.Solution;
        }
        public NotEscalatedViewModel(Incident i, DbInterfaceContainer dbInterfaces)
        {
            CurrentState = i.History.ActiveStatus.State;
            CurrentImpact = i.Impact;
            CurrentSubcategory = i.Category;
            CurrentUrgency = i.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
            FindUserEventArea = false;
            CloseRequestCommand = new CloseRequestCommand(dbInterfaces, this);
            Topic = i.Topic;
            Description = i.Description;
            Solution = i.History.Solution;
        }
        public NotEscalatedViewModel(ServiceRequest c, DbInterfaceContainer dbInterfaces)
        {
            CurrentState = c.History.ActiveStatus.State;
            CurrentImpact = c.Impact;
            CurrentSubcategory = c.Category;
            CurrentUrgency = c.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
            FindUserEventArea = false;
            CloseRequestCommand = new CloseRequestCommand(dbInterfaces, this);
            Topic = c.Topic;
            Description = c.Description;
            Solution = c.History.Solution;
        }
        public ICommand CloseRequestCommand { get; }
        public ICommand FindUserCommand { get; }
        private readonly IRequestQueue _requestQueue;
        public RequestViewModel RequestViewModel { get; set; }
        public List<State> States { get; private set; }
        public List<Urgency> Urgencies { get; private set; }
        public List<Categorization> Categorizations { get; private set; }
        public List<Impact> Impacts { get; private set; }
        public List<Priority> Priorities { get; private set; }
        public List<CloserDue> CloserDues { get; private set; }
        public string AdminUsername { get; set; }

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
        private Brush _cBorderBrush;
        private Brush _descBrush;
        private CloserDue _closerDue;
        private bool _findUserEvenArea;
        private Brush _statusBrush;
        private Brush _dueBrush;
        private Brush _solutionBrush;
        private Brush _titleBrush;
        private string _topic;
        private string _desc;
        private string _solution;

        public bool FindUserEventArea
        {
            get { return _findUserEvenArea; }
            set
            {
                _findUserEvenArea = value;
                OnPropertyChanged("FindUserEventArea");
            }

        }

        public Brush ContactValid
        {
            get { return _cBorderBrush; }
            set
            {
                if (value != _cBorderBrush)
                {
                    _cBorderBrush = value;
                    OnPropertyChanged("ContactValid");
                }
            }
        }
        public Brush RecipientValid
        {
            get { return _descBrush; }
            set
            {
                if (value != _descBrush)
                {
                    _descBrush = value;
                    OnPropertyChanged("RecipientValid");
                }
            }
        }
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
        public Brush StatusValid
        {
            get { return _statusBrush; }
            set
            {
                if (value != _statusBrush)
                {
                    _statusBrush = value;
                    OnPropertyChanged("StatusValid");
                }
            }
        }
        public Brush TitleValid
        {
            get { return _titleBrush; }
            set
            {
                if (value != _titleBrush)
                {
                    _titleBrush = value;
                    OnPropertyChanged("TitleValid");
                }
            }
        }
        public Brush DescriptionValid
        {
            get { return _descBrush; }
            set
            {
                if (value != _descBrush)
                {
                    _descBrush = value;
                    OnPropertyChanged("DescriptionValid");
                }
            }
        }
        public Brush CloserDueValid
        {
            get { return _dueBrush; }
            set
            {
                if (value != _dueBrush)
                {
                    _dueBrush = value;
                    OnPropertyChanged("CloserDueValid");
                }
            }
        }
        public Brush SolutionValid
        {
            get { return _solutionBrush; }
            set
            {
                if (value != _solutionBrush)
                {
                    _solutionBrush = value;
                    OnPropertyChanged("SolutionValid");
                }
            }
        }

        public CloserDue CloserDue
        {
            get { return _closerDue; }
            set
            {
                if (value != null)
                {
                    _closerDue = value;
                    OnPropertyChanged("CloserDue");
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
            get { return _priority; }
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
                Contact = null;
                OnPropertyChanged("CUsername");
            }
        }
        public string RUsername
        {
            get { return _rUsername; }
            set
            {
                _rUsername = value;
                Recipient = null;
                OnPropertyChanged("RUsername");
            }
        }

        public string Topic
        {
            get { return _topic; }
            set
            {
                _topic = value;
                OnPropertyChanged("Topic");
            }
        }
        public string Description
        {
            get { return _desc; }
            set
            {
                _desc = value;
                OnPropertyChanged("Description");
            }
        }
        public string Solution
        {
            get { return _solution; }
            set
            {
                _solution = value;
                OnPropertyChanged("Solution");
            }
        }

        public AccountInfo Contact
        {
            get { return _contact; }
            set
            {
                _contact = value;
                OnPropertyChanged("Contact");
            }
        }
        public AccountInfo Recipient
        {
            get { return _recipient; }
            set
            {
                _recipient = value;
                OnPropertyChanged("Recipient");
            }
        }

        public async Task Initialize()
        {
            States = await _requestQueue.GetAllStates();
            Categorizations = await _requestQueue.GetSubcategories();
            Urgencies = await _requestQueue.GetUrgencies();
            Impacts = await _requestQueue.GetImpacts();
            Priorities = await _requestQueue.GetPriority();
            CloserDues = await _requestQueue.GetCloserDues();
            CUsername = RequestViewModel.WorkspaceInfo[0].CommunicationInfo.RequestedPerson;
            RUsername = RequestViewModel.WorkspaceInfo[0].CommunicationInfo.ContactPerson;
            ContactValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            RecipientValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            AdminUsername = RequestViewModel.GetUsername();
            SetPriority();
        }
        public void SetInitialColors()
        {
            ContactValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            RecipientValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            StatusValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            TitleValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            DescriptionValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            CloserDueValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            SolutionValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        } 
        private void SetPriority()
        {
            if (CurrentUrgency != null && CurrentImpact != null && Priorities != null)
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
        public AccountInfo(Employee _user, string superiorUsername)
        {
            Firstname = _user.Firstname;
            Surname = _user.Surname;
            Localization = _user.Location.City;
            Position = _user.Positions.PositionName;
            SuperiorName = _user.Superior.Firstname;
            SuperiorSurname = _user.Superior.Surname;
            SuperiorEmail = _user.Superior.Email;
            SuperiorUsername = superiorUsername;
            DepartmentName = _user.Department.DepartmentName;
            Email = _user.Email;
        }
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
