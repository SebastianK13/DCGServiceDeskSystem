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
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ILoggedUser loggedUser;
        private int _currentTab;
        private bool locked = false;
        private bool _vis;

        public ICommand RequestCommand { get; }
        public ICommand UpdateGroupsCommand { get; }
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private readonly DbInterfaceContainer _interfaceContainer;
        private readonly ObservableCollection<ITab> _tabs;
        public IList<ITab> Tabs { get; }
        public int CurrentTabIndex
        {
            get { return _currentTab; }
            set
            {
                if (PreviousTabIndex != -1 && !locked)
                    PreviousTabIndex = -1;

                _currentTab = value;
                Vis = false;
                Vis = true;

                locked = false;
                OnPropertyChanged("CurrentTabIndex");
            }
        }
        public int PreviousTabIndex { get; set; }
        public bool Vis
        {
            get { return _vis; }
            set
            {
                _vis = value;
                OnPropertyChanged("Vis");
            }
        }
        public ICollection<AssigmentGroup> Groups { get; set; }

        public HomeViewModel(ILoggedUser loggedUser, DbInterfaceContainer interfaceContainer)
        {
            _tabs = new ObservableCollection<ITab>();
            _tabs.CollectionChanged += Tabs_CollectionChanged;

            Tabs = _tabs;

            this.loggedUser = loggedUser;
            _requestQueue = interfaceContainer.RequestQueue;
            _userInfo = interfaceContainer.UserInfo;
            _employeeProfile = interfaceContainer.EmployeeProfile;
            _interfaceContainer = new DbInterfaceContainer(_requestQueue,_userInfo, _employeeProfile);

            RequestCommand = new RequestCommand(_requestQueue, _userInfo, _employeeProfile, this);           
            UpdateGroupsCommand = new UpdateGroupsCommand(this, _requestQueue);
            Groups = _requestQueue.GetUserGroupsNotAsync(loggedUser.ActiveUser);
        }
        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ITab tab;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int temp = CurrentTabIndex;
                    tab = (ITab)e.NewItems[0];
                    tab.CloseTabRequested += TabCloseRequested;
                    CurrentTabIndex = Tabs.Count-1;
                    PreviousTabIndex = temp;
                    locked = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    tab = (ITab)e.OldItems[0];
                    tab.CloseTabRequested -= TabCloseRequested;
                    if (CurrentTabIndex == e.OldStartingIndex)
                        CheckTabs();
                    else if (PreviousTabIndex != -1)
                        CurrentTabIndex = PreviousTabIndex;
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

        public void SetSearchingTab() =>
            Tabs.Add(new RequestSearchViewModel(_interfaceContainer, this));

        public void CloseTab(ITab tab) =>
            Tabs.Remove(tab);

        public void RemoveFromQueue(object request)
        {
            string requestType = request.GetType().Name;
            var choosenRequest = RequestService.ConvertRequest(request, requestType);
            int y = 0;
            foreach (var r in Tabs)
            {
                if (r.GetType().Name == "QueueViewModel")
                {
                    foreach (var t in r.WorkspaceInfo)
                    {
                        string currentType = t.ServiceRequests?.GetType().Name;

                        if (currentType == requestType)
                        {
                            var currentRequest = RequestService.ConvertRequest(t.ServiceRequests, requestType);

                            int toRemoveID = RequestService.GetId(choosenRequest);
                            int currentID = RequestService.GetId(currentRequest);

                            if (toRemoveID == currentID)
                            {
                                r.WorkspaceInfo.Remove(t);
                                y++;
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void CheckTabs()
        {

            if (Tabs.Count < CurrentTabIndex + 1)
                CurrentTabIndex -= 1;
            else
            {
                CurrentTabIndex = -1;
                PreviousTabIndex = -1;
            }
        } 
    }
}
