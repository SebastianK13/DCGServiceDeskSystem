using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.CurrentUser;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ILoggedUser loggedUser;

        public ICommand RequestCommand { get; }
        private readonly IRequestQueue _requestQueue;
        private readonly ObservableCollection<ITab> _tabs;
        public ICollection<ITab> Tabs { get; }

        public HomeViewModel(ILoggedUser loggedUser, IRequestQueue requestQueue)
        {
            _tabs = new ObservableCollection<ITab>();
            _tabs.CollectionChanged += Tabs_CollectionChanged;

            Tabs = _tabs;

            this.loggedUser = loggedUser;
            _requestQueue = requestQueue;

            RequestCommand = new RequestCommand(_requestQueue, this);
        }
        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ITab tab;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    tab = (ITab)e.NewItems[0];
                    tab.CloseTabRequested += OnTabCloseRequested;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    tab = (ITab)e.OldItems[0];
                    tab.CloseTabRequested -= OnTabCloseRequested;
                    break;
            }
        }
        private void OnTabCloseRequested(object sender, EventArgs e) =>
            Tabs.Remove((ITab)sender);

        public void SetRequests(List<object> requests) =>
            Tabs.Add(new TabContent(requests, "Service Requests"));
        public void SetIncidents(List<Incident> incidents) =>
            Tabs.Add(new TabContent(incidents, "Incidents"));
        public void SetChanges(List<ServiceRequest> changes) =>
            Tabs.Add(new TabContent(changes, "Changes"));
        public void SetTasks(List<TaskRequest> tasks) =>
            Tabs.Add(new TabContent(tasks, "Tasks"));

    }
}
