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
        protected readonly TimeZoneInfo _timeZoneInfo;
        public RequestViewModel(SingleRequestInfo singleRequest,
            DbInterfaceContainer interfaceContainer, HomeViewModel hVM, bool isEscalated)
            : base(interfaceContainer, hVM)
        {
            _timeZoneInfo = hVM.loggedUser.ZoneInfo;
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = singleRequest.Request;
            wi.CommunicationInfo = singleRequest.Info;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = singleRequest.Label;
            wi.RequestType = singleRequest.Request.GetType().Name;
            NotEscalated = new NotEscalatedViewModel(RequestService.ConvertRequest(singleRequest.Request),
                interfaceContainer, this);
            Escalation = new EscalationViewModel(this, interfaceContainer.RequestQueue)
            {
                AssigmentGroups = singleRequest.Groups
            };
            if (isEscalated)
            {
                Escalated = new EscalatedRequestViewModel(this, NotEscalated);
                Escalated.SLA = DateTimeConversion.ConvertDate(
                    RequestService.GetSLADate(singleRequest.Request), _timeZoneInfo);
                Escalated.AssigmentGroups = singleRequest.Groups;
                Escalated.AUsername = RequestService.GetAssignee(singleRequest.Request);
                Escalated.ChoosenGroup = RequestService.GetGroup(singleRequest.Request);
                CurrentMode = Escalated;
            }
            else
            {
                CurrentMode = NotEscalated;
            }
        }
        public async Task InitializeNEVMModel()
        {
            await NotEscalated.Initialize();
            await NotEscalated.CheckIfRequestAssigned();
            NotEscalated.SetInitialColors();
        }
        public async Task InitializeEVMModel()
        {
            await Escalation.Initialize();
        }
        public void InitializeEscalatedModel()
        {
            Escalated.SetInitialColors();
            Escalated.SetStatuses();
        }

    }
    public class EscalatedRequestViewModel : ViewModelBase
    {

        public EscalatedRequestViewModel(RequestViewModel requestViewModel, NotEscalatedViewModel notEscalated)
        {
            RequestViewModel = requestViewModel;
            NotEscalated = notEscalated;
            NotEscalated.ButtonsVisibile = true;
            //NotEscalated.States
            //    .Remove(NotEscalated.States
            //    .Where(n => n.StateName == "New")
            //    .FirstOrDefault());

            PickMemberCommand = new PickMemberCommand(this);
        }

        public ICommand PickMemberCommand { get; }
        private AssigmentGroup _chosenGroup;
        private string _aUsername;
        private DateTime _sla;
        private AccountInfo _assignee;
        private bool _assigneeArea;
        private Brush _assigneeValid;
        private List<MemberInfo> _members;
        private bool _membersVisibility;

        public NotEscalatedViewModel NotEscalated { get; set; }
        public RequestViewModel RequestViewModel { get; set; }

        public List<AssigmentGroup> AssigmentGroups { get; set; }
        public List<MemberInfo> Members
        {
            get { return _members; }
            set
            {
                _members = value;
                OnPropertyChanged("Members");
            }
        }
        public AssigmentGroup ChoosenGroup
        {
            get { return _chosenGroup; }
            set
            {
                _chosenGroup = value;
                OnPropertyChanged("ChoosenGroup");
            }
        }
        public string AUsername
        {
            get { return _aUsername; }
            set
            {
                _aUsername = value;
                OnPropertyChanged("AUsername");
            }
        }
        public DateTime SLA
        {
            get { return _sla; }
            set
            {
                _sla = value;
                OnPropertyChanged("SLA");
            }
        }
        public AccountInfo Assignee
        {
            get { return _assignee; }
            set
            {
                _assignee = value;
                OnPropertyChanged("Assignee");
            }
        }
        public bool FindAssigneeEventArea
        {
            get { return _assigneeArea; }
            set
            {
                _assigneeArea = value;
                OnPropertyChanged("FindAssigneeEventArea");
            }
        }
        public bool MembersVisibility
        {
            get { return _membersVisibility; }
            set
            {
                _membersVisibility = value;
                OnPropertyChanged("MembersVisibility");
            }
        }
        public Brush AssigneeValid
        {
            get { return _assigneeValid; }
            set
            {
                if (value != _assigneeValid)
                {
                    _assigneeValid = value;
                    OnPropertyChanged("AssigneeValid");
                }
            }
        }
        public List<Notification> Notifications { get; set; }
        public List<Status> Statuses { get; set; }
        public void SetInitialColors() =>
            AssigneeValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        public void SetStatuses() =>
            Statuses = RequestService.GetStatuses(RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests);
    }
    public class NotEscalatedViewModel : ViewModelBase
    {
        public NotEscalatedViewModel(TaskRequest t, DbInterfaceContainer dbInterfaces, RequestViewModel rVM)
        {
            RequestViewModel = rVM;
            CurrentState = t.History.ActiveStatus.State;
            CurrentImpact = t.Impact;
            CurrentSubcategory = t.Category;
            CurrentUrgency = t.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
            FindUserEventArea = false;
            CloseOrEscalateCommand = new CloseOrEscalateCommand(dbInterfaces, this);
            Topic = t.Topic;
            Description = t.Description;
            Solution = t.History.Solution;
            _designation = "T";
            UpdateRequestCommand = new UpdateRequestCommand(_requestQueue, this);
            _requestId = t.TaskId;
            TaskInfo = t.AccountForm;
        }
        public NotEscalatedViewModel(Incident i, DbInterfaceContainer dbInterfaces, RequestViewModel rVM)
        {
            RequestViewModel = rVM;
            CurrentState = i.History.ActiveStatus.State;
            CurrentImpact = i.Impact;
            CurrentSubcategory = i.Category;
            CurrentUrgency = i.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
            FindUserEventArea = false;
            CloseOrEscalateCommand = new CloseOrEscalateCommand(dbInterfaces, this);
            Topic = i.Topic;
            Description = i.Description;
            Solution = i.History.Solution;
            _designation = "IM";
            UpdateRequestCommand = new UpdateRequestCommand(_requestQueue, this);
            _requestId = i.IncidentId;
        }
        public NotEscalatedViewModel(ServiceRequest c, DbInterfaceContainer dbInterfaces, RequestViewModel rVM)
        {
            RequestViewModel = rVM;
            CurrentState = c.History.ActiveStatus.State;
            CurrentImpact = c.Impact;
            CurrentSubcategory = c.Category;
            CurrentUrgency = c.Urgency;
            _requestQueue = dbInterfaces.RequestQueue;
            FindUserCommand = new FindUserCommand(dbInterfaces, this);
            FindUserEventArea = false;
            CloseOrEscalateCommand = new CloseOrEscalateCommand(dbInterfaces, this);
            Topic = c.Topic;
            Description = c.Description;
            Solution = c.History.Solution;
            _designation = "C";
            UpdateRequestCommand = new UpdateRequestCommand(_requestQueue, this);
            _requestId = c.RequestId;
        }
        public ICommand UpdateRequestCommand { get; }
        public ICommand CloseOrEscalateCommand { get; }
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
        public NewAccountForm TaskInfo { get; set; }

        private readonly int _requestId;
        private readonly string _designation;
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
        private bool _buttonsVisible;
        private bool _assignBtnVisible;
        private bool _taskInfoVisibility;
        private bool _isAccountInfoChecked;

        public bool AssignBtnVisibile
        {
            get { return _assignBtnVisible; }
            set
            {
                _assignBtnVisible = value;
                OnPropertyChanged("AssignBtnVisibile");
            }
        }
        public bool ButtonsVisibile
        {
            get { return _buttonsVisible; }
            set
            {
                _buttonsVisible = value;
                OnPropertyChanged("ButtonsVisibile");
            }
        }
        public bool TaskInfoVisibility
        {
            get { return _taskInfoVisibility; }
            set
            {
                _taskInfoVisibility = value;
                OnPropertyChanged("TaskInfoVisibility");
            }
        }
        public bool IsAccountInfoChecked
        {
            get { return _isAccountInfoChecked; }
            set
            {
                _isAccountInfoChecked = value;
                OnPropertyChanged("IsAccountInfoChecked");
            }
        }
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
            Categorizations = await _requestQueue.GetSubcategories(_designation);
            Urgencies = await _requestQueue.GetUrgencies();
            Impacts = await _requestQueue.GetImpacts();
            Priorities = await _requestQueue.GetPriority();
            CloserDues = await _requestQueue.GetCloserDues();
            CUsername = RequestViewModel.WorkspaceInfo.FirstOrDefault().CommunicationInfo.RequestedPerson;
            RUsername = RequestViewModel.WorkspaceInfo.FirstOrDefault().CommunicationInfo.ContactPerson;
            ContactValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            RecipientValid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
            AdminUsername = RequestViewModel.GetUsername();
            AssignBtnVisibile = true;
            SetPriority();
            TaskInfoVisibility = CheckIsTaskRequest();
        }
        public async Task CheckIfRequestAssigned()
        {
            string assignee = null;
            switch (_designation)
            {
                case "C":
                    assignee = await _requestQueue.GetChangeAssignee(_requestId);
                    break;
                case "T":
                    assignee = await _requestQueue.GetTaskAssignee(_requestId);
                    break;
                case "IM":
                    assignee = await _requestQueue.GetIncidentAssignee(_requestId);
                    break;
            }

            if (AdminUsername == assignee)
            {
                ButtonsVisibile = true;
                AssignBtnVisibile = false;
                IsAccountInfoChecked = false;
            }

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
        private bool CheckIsTaskRequest() =>
            TaskInfo != null ? true : false;

    }
    public class EscalationViewModel : ViewModelBase
    {
        public EscalationViewModel(RequestViewModel rVM, IRequestQueue requestQueue)
        {
            RequestViewModel = rVM;
            _requestQueue = requestQueue;
            designation = rVM.WorkspaceInfo.FirstOrDefault().ServiceRequests.GetType().Name;
            EscalateRequestCommand = new EscalateRequestCommand(rVM, requestQueue);
            ForwardOrDropCommand = new ForwardOrDropCommand(requestQueue, this);
        }
        private readonly IRequestQueue _requestQueue;
        private AssigmentGroup _chosenGroup;
        public string designation;
        private bool _isAssociated;
        private bool _openIncidentsVisibility;
        private OpenIncidentContainer _choosenIncident;

        public AssigmentGroup ChoosenGroup
        {
            get { return _chosenGroup; }
            set
            {
                _chosenGroup = value;
                OnPropertyChanged("ChoosenGroup");
            }
        }
        public bool IsAssociated
        {
            get { return _isAssociated; }
            set
            {
                _isAssociated = value;
                OnPropertyChanged("isAssociated");
            }
        }
        public bool OpenIncidentsVisibility
        {
            get { return _openIncidentsVisibility; }
            set
            {
                _openIncidentsVisibility = value;
                OnPropertyChanged("OpenIncidentsVisibility");
            }
        }

        public bool Visibility { get; set; }
        public ICommand ForwardOrDropCommand { get; set; }
        public ICommand EscalateRequestCommand { get; }
        public List<AssigmentGroup> AssigmentGroups { get; set; }
        public List<OpenIncidentContainer> OpenIncidents { get; set; } = new List<OpenIncidentContainer>();
        public RequestViewModel RequestViewModel { get; set; }
        public OpenIncidentContainer ChoosenIncident
        {
            get { return _choosenIncident; }
            set
            {
                _choosenIncident = value;
                OnPropertyChanged("ChoosenIncident");
            }
        }
        public async Task Initialize()
        {
            if (designation == "IncidentProxy")
            {
                Visibility = true;
                var incidents = await _requestQueue.GetOpenIncidents();

                foreach (var i in incidents)
                {
                    OpenIncidents.Add(new OpenIncidentContainer
                    {
                        Incident = i,
                        IncidentID = RequestService.SetRequestId(i.IncidentId, "IM")
                    });
                }
            }
            else
            {
                Visibility = false;
            }
        }
    }

    public class AccountInfo
    {
        public AccountInfo()
        {
        }

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
    public class OpenIncidentContainer
    {
        public Incident Incident { get; set; }
        public string IncidentID { get; set; }
    }
    public class MemberInfo
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
    }
}
