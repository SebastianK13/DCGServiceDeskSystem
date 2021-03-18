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
    public abstract class Tab : ITab
    {
        public Tab(IRequestQueue requestQueue,
            IUserInfo userInfo, IEmployeeProfile employeeProfile)
        {
            _requestQueue = requestQueue;
            _userInfo = userInfo;
            _employeeProfile = employeeProfile;
            CloseTabCommand = new ActionCommand(p => CloseTabRequested?.Invoke(this, EventArgs.Empty));
            IViewRequestService vRS =
                new ViewRequestService(_requestQueue, _userInfo, _employeeProfile);
            RefreshCommand = new RefreshTabCommand(this, vRS, _requestQueue);
        }
        public List<TabContainer> WorkspaceInfo { get; set; } = new List<TabContainer>();
        public string Label { get; set; }
        public string QueueType { get; set; }
        public ICommand CloseTabCommand { get; }
        public object CloseRequested { get; }
        public event EventHandler CloseTabRequested;
        public ICommand RefreshCommand { get; }
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;

    }
    public class TabContainer
    {
        public object ServiceRequests { get; set; } = new List<object>();
        public CommunicationInfo CommunicationInfo { get; set; }
        public bool RequestVisibility { get; set; }
        public string RequestType { get; set; }
    }

}
