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

        public static dynamic ConvertRequest(List<object> requests,string requestType)
        {
            switch (requestType)
            {
                case "Tasks":
                    return requests.Cast<TaskRequest>().ToList();
                case "Incidents":
                    return requests.Cast<Incident>().ToList();
                case "Changes":
                    return requests.Cast<Incident>().ToList();
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
    }
    public class SingleRequestInfo
    {
        public object Request { get; set; }
        public CommunicationInfo Info { get; set; }
        public List<State> States { get; set; }
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
