using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.ViewModels
{
    public class QueueViewModel : Tab
    {
        private List<string> requestTypes = new List<string> { "Incidents", "Changes", "Tasks" };
        public QueueViewModel(RequestInfo requestInfo, IRequestQueue requestQueue,
            IUserInfo userInfo, IEmployeeProfile employeeProfile):base(requestQueue,
             userInfo,  employeeProfile)
        {
            if (!requestTypes.Contains(requestInfo.QueueName))
                MixedTypeQueue(requestInfo);
            else
                SingleTypeQueue(requestInfo);

        }

        private object ConvertType(string requestType, object request)
        {
            switch (requestType)
            {
                case "Incidents":
                    return (Incident)request;
                case "Changes":
                    return (ServiceRequest)request;
                case "Tasks":
                    return (TaskRequest)request;
                default:
                    return null;
            }
        }
        private void SingleTypeQueue(RequestInfo requestInfo)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for (int i = 0; i < requestInfo.Requests.Count; i++)
            {
                wi.Add(new TabContainer
                {
                    CommunicationInfo = requestInfo.Infos[i],
                    RequestVisibility = true,
                    ServiceRequests = ConvertType(requestInfo.RequestTypes[0], requestInfo.Requests[i])
                });
            }

            WorkspaceInfo = wi;
            Label = requestInfo.QueueName;
            QueueType = requestInfo.QueueType;
        }
        private void MixedTypeQueue(RequestInfo requestInfo)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for (int i = 0; i < requestInfo.Requests.Count; i++)
            {
                wi.Add(new TabContainer
                {
                    CommunicationInfo = requestInfo.Infos[i],
                    RequestVisibility = true,
                    ServiceRequests = ConvertType(requestInfo.RequestTypes[i], requestInfo.Requests[i]),
                    RequestType = requestInfo.RequestTypes[i]
                });
            }
            WorkspaceInfo = wi;
            Label = requestInfo.QueueName;
            QueueType = requestInfo.QueueType;
        }
    }
}
