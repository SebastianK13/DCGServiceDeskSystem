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
        public static DateTime GetSLADate(object request)
        {
            string requestType = request.GetType().Name;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    return ((TaskRequest)request).History.ActiveStatus.DueTime;
                case "IncidentProxy":
                    return ((Incident)request).History.ActiveStatus.DueTime;
                case "ServiceRequestProxy":
                    return ((ServiceRequest)request).History.ActiveStatus.DueTime;
                default:
                    return new DateTime();

            }

        }
        public static string GetAssignee(object request) 
        {
            string requestType = request.GetType().Name;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    return ((TaskRequest)request).Assignee;
                case "IncidentProxy":
                    return ((Incident)request).Assignee;
                case "ServiceRequestProxy":
                    return ((ServiceRequest)request).Assignee;
                default:
                    return null;

            }
        }
        public static AssigmentGroup GetGroup(object request)
        {
            string requestType = request.GetType().Name;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    return ((TaskRequest)request).Group;
                case "IncidentProxy":
                    return ((Incident)request).Group;
                case "ServiceRequestProxy":
                    return ((ServiceRequest)request).Group;
                default:
                    return null;

            }
        }
        public static List<Status> GetStatuses(object request)
        {
            string requestType = request.GetType().Name;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    return ((TaskRequest)request).History.Status.ToList();
                case "IncidentProxy":
                    return ((Incident)request).History.Status.ToList();
                case "ServiceRequestProxy":
                    return ((ServiceRequest)request).History.Status.ToList();
                default:
                    return null;

            }
        }
        public static TaskRequest SetAssigmentGroupT(TaskRequest task, AssigmentGroup group)
        {
            task.Group = group;
            return task;
        }

        public static Incident SetAssigmentGroupIM(Incident incident, AssigmentGroup group)
        {
            incident.Group = group;
            return incident;
        }

        public static ServiceRequest SetAssigmentGroupC(ServiceRequest change, AssigmentGroup group)
        {
            change.Group = group;
            return change;
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
    public class Notification
    {
        public DateTime CreateDate { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string GroupName { get; set; }


        public List<Notification> NotificationBuilder(List<Status> statuses)
        {
            List<Notification> notifications = new List<Notification>();

            for(int i = 0; i < statuses.Count; i++)
            {
                string option = statuses[i].State.StateName;
                Notification temp = new Notification();
                temp.CreateDate = statuses[i].CreateDate;
                temp.AssignedTo = statuses[i].AssignedTo;
                temp.CreatedBy = statuses[i].CreatedBy;
                temp.GroupName = statuses[i].Group.GroupName;

                switch (option)
                {
                    case "New":
                        temp.Message = "New request has been registered by " +
                             CreatedBy + " and Escalated to " + GroupName;
                        break;
                    case "Open":
                        if(CreatedBy == null)
                            temp.Message = "Request has been opened automate";
                        else
                            temp.Message = "Request has been assigned to "+GroupName+" by "+ CreatedBy;
                        break;
                    case "Waiting":
                        temp.Message = "Request is waiting for reply. Status has been changed by " + CreatedBy;
                        break;
                    case "Closed":
                        temp.Message = "Request has been closed by " + CreatedBy;
                        break;
                }

                notifications.Add(temp);
            }

            return notifications;
        }
    }
}
