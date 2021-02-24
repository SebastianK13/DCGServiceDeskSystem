using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class RequestViewModel:Tab
    {
        public RequestViewModel(ServiceRequest changes, string label, CommunicationInfo communicationInfo)
        {
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = changes;
            wi.CommunicationInfo = communicationInfo;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = label;
        }
        public RequestViewModel(Incident incidents, string label, CommunicationInfo communicationInfo)
        {
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = incidents;
            wi.CommunicationInfo = communicationInfo;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = label;
        }
        public RequestViewModel(TaskRequest tasks, string label, CommunicationInfo communicationInfo)
        {
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = tasks;
            wi.CommunicationInfo = communicationInfo;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = label;
        }
    }
}
