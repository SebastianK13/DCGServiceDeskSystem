using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.ViewModels
{
    public class EscalatedViewModel:Tab
    {
        public List<State> States { get; set; }
        public EscalatedViewModel(ServiceRequest changes, string label, CommunicationInfo communicationInfo, List<State> states)
        {
            States = states;
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = changes;
            wi.CommunicationInfo = communicationInfo;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = label;
        }
        public EscalatedViewModel(Incident incidents, string label, CommunicationInfo communicationInfo, List<State> states)
        {
            States = states;
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = incidents;
            wi.CommunicationInfo = communicationInfo;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = label;
        }
        public EscalatedViewModel(TaskRequest tasks, string label, CommunicationInfo communicationInfo, List<State> states)
        {
            States = states;
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = tasks;
            wi.CommunicationInfo = communicationInfo;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = label;
        }
    }
}
