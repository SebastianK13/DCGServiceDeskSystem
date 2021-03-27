using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCGServiceDesk.Services
{
    public static class RequestService
    {
        public static List<object> ConvertTotListObject(object list) =>
            (list as IEnumerable<object>).Cast<object>().ToList();

        public static dynamic ConvertRequest(object request,string requestType = "")
        {
            if (requestType == "")
                requestType = request.GetType().Name;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    return (TaskRequest)request;
                case "IncidentProxy":
                    return (Incident)request;
                case "ServiceRequestProxy":
                    return (ServiceRequest)request;
                default:
                    return null;

            }
        }
        public static dynamic GetId(object request)
        {
            switch (request.GetType().Name)
            {
                case "TaskRequestProxy":
                    return ((TaskRequest)request).TaskId;
                case "Incidents":
                    return ((Incident)request).IncidentId;
                case "Changes":
                    return ((ServiceRequest)request).RequestId;
                default:
                    return null;

            }
        }
        public static List<CommunicationInfo> AddRequestIds(List<CommunicationInfo> info, List<int> Ids, string requestType)
        {
            for (int i = 0; i < info.Count; i++)
            {
                int temp = 1000000 + Ids[i];
                info[i].RequestId = requestType + temp.ToString().Substring(1);
            }

            return info;
        }
        public static string SetRequestId(int id, string requestType)
        {
            int temp = 1000000 + id;
            return requestType + temp.ToString().Substring(1);
        }
        public static List<CommunicationInfo> AddRequestIdsMixed(List<CommunicationInfo> info, List<int> Ids, List<string> requestTypes)
        {
            for (int i = 0; i < info.Count; i++)
            {
                int temp = 1000000 + Ids[i];
                info[i].RequestId = SetShortcut(requestTypes[i]) + temp.ToString().Substring(1);
            }

            return info;
        }
        private static string SetShortcut(string typeName)
        {
            switch (typeName)
            {
                case "Tasks":
                    return "T";
                case "Incidents":
                    return "IM";
                case "Changes":
                    return "C";
            }

            return "SD";
        }
        public static Incident ExtractIncident(object parameter) =>
            (Incident)((TabContainer)parameter).ServiceRequests;

        public static ServiceRequest ExtractChange(object parameter) =>
            (ServiceRequest)((TabContainer)parameter).ServiceRequests;
        public static TaskRequest ExtractTask(object parameter) =>
            (TaskRequest)((TabContainer)parameter).ServiceRequests;

        public static CommunicationInfo ExtractAdditionalInfo(object parameter) =>
            ((TabContainer)parameter).CommunicationInfo;

        public static int GetPriorityLvl(int urgency, int impact)
        {
            switch (urgency)
            {
                case 1:
                    switch (impact)
                    {
                        case int i when (i <= 2):
                            return 1;
                        case int i when (i > 3):
                            return 2;
                    }
                    break;
                case 2:
                    switch (impact)
                    {
                        case 1:
                            return 1;
                        case int i when (i == 2 || i == 3):
                            return 2;
                        case 4:
                            return 3;
                    }
                    break;
                case 3:
                    switch (impact)
                    {
                        case 1:
                            return 2;
                        case int i when (i > 1):
                            return 3;
                    }
                    break;
                case 4:
                    return 4;
            }
            return 0;
        }
    }
    public class SingleRequestInfo
    {
        public object Request { get; set; }
        public CommunicationInfo Info { get; set; }
        public List<State> States { get; set; }
        public List<AssigmentGroup> Groups { get; set; }
        public string Label { get; set; }
    }
    public class RequestInfo
    {
        public List<object> Requests { get; set; }
        public List<CommunicationInfo> Infos { get; set; }
        public List<string> RequestTypes { get; set; }
        public string QueueName { get; set; }
        public string QueueType { get; set; }
    }
}
