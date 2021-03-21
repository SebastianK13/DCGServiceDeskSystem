using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.CurrentUser;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ILoggedUser loggedUser;

        public ICommand RequestCommand { get; }
        public ICommand UpdateRequestCommand { get; }
        public ICommand UpdateGroupsCommand { get; }
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private readonly DbInterfaceContainer _interfaceContainer;
        private readonly ObservableCollection<ITab> _tabs;
        public IList<ITab> Tabs { get; }
        public ICollection<AssigmentGroup> Groups { get; }

        public HomeViewModel(ILoggedUser loggedUser, IRequestQueue requestQueue, IUserInfo userInfo, IEmployeeProfile employeeProfile)
        {
            _tabs = new ObservableCollection<ITab>();
            _tabs.CollectionChanged += Tabs_CollectionChanged;

            Tabs = _tabs;

            this.loggedUser = loggedUser;
            _requestQueue = requestQueue;
            _userInfo = userInfo;
            _employeeProfile = employeeProfile;
            _interfaceContainer = new DbInterfaceContainer(_requestQueue,_userInfo, _employeeProfile);

            RequestCommand = new RequestCommand(_requestQueue, _userInfo, _employeeProfile, this);
            UpdateRequestCommand = new UpdateRequestCommand(this, _requestQueue, loggedUser.ActiveUser);
            UpdateGroupsCommand = new UpdateGroupsCommand(this, _requestQueue);
            Groups = _requestQueue.GetGroups(loggedUser.ActiveUser);
        }
        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ITab tab;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    tab = (ITab)e.NewItems[0];
                    tab.CloseTabRequested += TabCloseRequested;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    tab = (ITab)e.OldItems[0];
                    tab.CloseTabRequested -= TabCloseRequested;
                    break;
            }
        }
        private void TabCloseRequested(object sender, EventArgs e) =>
            Tabs.Remove((ITab)sender);

        public void TabRefresh(ITab tab, RequestInfo requestInfo)
        {
            int index = Tabs.IndexOf(tab);
            Tabs[index] = new QueueViewModel(requestInfo, _interfaceContainer, this);
        }

        public void SetRequestsQueue(RequestInfo requestInfo) =>
            Tabs.Add(new QueueViewModel(requestInfo, _interfaceContainer, this));

    }
}
