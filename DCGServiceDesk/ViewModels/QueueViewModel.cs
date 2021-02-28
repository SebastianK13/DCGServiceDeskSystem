using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.ViewModels
{
    public class QueueViewModel:Tab
    {
        public QueueViewModel(List<ServiceRequest> changes, string label, List<CommunicationInfo> communicationInfo)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for(int i = 0; i < changes.Count; i++)
            {
                wi.Add(new TabContainer { CommunicationInfo = communicationInfo[i],
                RequestVisibility = true,
                ServiceRequests = changes[i] });
            }
            WorkspaceInfo = wi;
            Label = label;
        }
        public QueueViewModel(List<Incident> incidents, string label, List<CommunicationInfo> communicationInfo)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for (int i = 0; i < incidents.Count; i++)
            {
                wi.Add(new TabContainer
                {
                    CommunicationInfo = communicationInfo[i],
                    RequestVisibility = true,
                    ServiceRequests = incidents[i]
                });
            }
            WorkspaceInfo = wi;
            Label = label;
        }
        public QueueViewModel(List<TaskRequest> tasks, string label, List<CommunicationInfo> communicationInfo)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for (int i = 0; i < tasks.Count; i++)
            {
                wi.Add(new TabContainer
                {
                    CommunicationInfo = communicationInfo[i],
                    RequestVisibility = true,
                    ServiceRequests = tasks[i]
                });
            }
            WorkspaceInfo = wi;
            Label = label;
        }
        public QueueViewModel(List<object> requests, string label, List<CommunicationInfo> communicationInfo, List<string> RequestTypes)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for (int i = 0; i < requests.Count; i++)
            {
                wi.Add(new TabContainer
                {
                    CommunicationInfo = communicationInfo[i],
                    RequestVisibility = true,
                    ServiceRequests = requests[i],
                    RequestType = RequestTypes[i]
                });
            }
            WorkspaceInfo = wi;
            Label = label;
        }
    }
}
