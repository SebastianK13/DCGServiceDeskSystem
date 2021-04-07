using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCGServiceDesk.Controls.Tab.Model
{
    public abstract class Tab : ViewModelBase, ITab
    {
        public Tab(DbInterfaceContainer interfaceContainer, HomeViewModel homeViewModel)
        {
            HVM = homeViewModel;
            _requestQueue = interfaceContainer.RequestQueue;
            _userInfo = interfaceContainer.UserInfo;
            _employeeProfile = interfaceContainer.EmployeeProfile;
            CloseTabCommand = new CloseTabCommand(HVM);
            IViewRequestService vRS =
                new ViewRequestService(interfaceContainer);
            RefreshCommand = new RefreshTabCommand(this, vRS, _requestQueue);
        }
        private HomeViewModel HVM { get; set; }
        public List<TabContainer> WorkspaceInfo { get; set; } = new List<TabContainer>();
        public string Label { get; set; }
        public string QueueType { get; set; }
        public ICommand CloseTabCommand { get; }
        public ICommand RefreshCommand { get; }
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        public event EventHandler CloseTabRequested;
        public NotEscalatedViewModel NotEscalated { get; set; }
        public EscalationViewModel Escalation { get; set; }
        private object _currentMode;
        public object CurrentMode
        {
            get { return _currentMode; }
            set
            {
                if (_currentMode != value)
                {
                    _currentMode = value;
                    OnPropertyChanged("CurrentMode");
                }
            }
        }
        public string GetUsername() =>
            HVM.loggedUser.ActiveUser;
        public void RemoveAssignedRequest(object request) =>
            HVM.RemoveFromQueue(request);
        public void RemoveCurrentTab() =>
            HVM.Tabs.Remove(this);

    }
    public class TabContainer
    {
        public object ServiceRequests { get; set; } = new List<object>();
        public CommunicationInfo CommunicationInfo { get; set; }
        public bool RequestVisibility { get; set; }
        public string RequestType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DeadlineDate { get; set; }
    }

}
