using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DCGServiceDesk.Services.Notification;

namespace DCGServiceDesk.Services
{
    public static class RequestService
    {
        public static List<object> ConvertTotListObject(object list) =>
            (list as IEnumerable<object>).Cast<object>().ToList();

        public static dynamic ConvertRequest(object request, string requestType = "")
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
        public static string GetStateName(object request)
        {
            string requestType = request.GetType().Name;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    return ((TaskRequest)request).History.ActiveStatus.State.StateName;
                case "IncidentProxy":
                    return ((Incident)request).History.ActiveStatus.State.StateName;
                case "ServiceRequestProxy":
                    return ((ServiceRequest)request).History.ActiveStatus.State.StateName;
                default:
                    return null;

            }
        }
        public static int GetHistoryId(object request)
        {
            string requestType = request.GetType().Name;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    return ((TaskRequest)request).HistoryId;
                case "IncidentProxy":
                    return ((Incident)request).HistoryId;
                case "ServiceRequestProxy":
                    return ((ServiceRequest)request).HistoryId;
                default:
                    return 0;

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
                case "IncidentProxy":
                    return ((Incident)request).IncidentId;
                case "ServiceRequestProxy":
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
        public static List<Change> FindChanges(object originalRequest, object currentRequest, List<string>usernames)
        {
            string requestType = originalRequest.GetType().Name;
            List<Change> Changes = new List<Change>();
            switch (requestType)
            {
                case "TaskRequestProxy":
                    TaskRequest original = (TaskRequest)originalRequest;
                    TaskRequest current = (TaskRequest)currentRequest;
                    if (original.ContactPerson != current.ContactPerson)
                        Changes.Add(new Change
                        { 
                            PropertyName = "Recipient", 
                            Original = usernames[0], 
                            Current = usernames[2]
                        });
                    if (original.RequestedPerson != current.RequestedPerson)
                        Changes.Add(new Change 
                        { 
                            PropertyName = "Contact", 
                            Original = usernames[1], 
                            Current = usernames[3] 
                        });
                    if (original.GroupId != current.GroupId)
                        Changes.Add(new Change
                        {
                            Original = original.Group.GroupName,
                            Current = current.Group.GroupName,
                            PropertyName = "Assigment group"
                        });
                    if (original.Assignee != current.Assignee)
                        Changes.Add(new Change
                        {
                            Original = original.Assignee,
                            Current = current.Assignee,
                            PropertyName = "Assignee"
                        });
                    if (original.ImpactId != current.ImpactId)
                        Changes.Add(new Change
                        {
                            Original = original.ImpactId.ToString(),
                            Current = current.ImpactId.ToString(),
                            PropertyName = "Impact"
                        });
                    if (original.UrgencyId != current.UrgencyId)
                        Changes.Add(new Change
                        {
                            Original = original.UrgencyId.ToString(),
                            Current = current.UrgencyId.ToString(),
                            PropertyName = "Urgency"
                        });
                    if (original.Topic != current.Topic)
                        Changes.Add(new Change
                        {
                            Original = original.Topic,
                            Current = current.Topic,
                            PropertyName = "Topic"
                        });
                    if (original.Description != current.Description)
                        Changes.Add(new Change
                        {
                            Original = original.Description,
                            Current = current.Description,
                            PropertyName = "Description"
                        });
                    break;
                case "IncidentProxy":
                    Incident originalIM = (Incident)originalRequest;
                    Incident currentIM = (Incident)currentRequest;
                    if (originalIM.ContactPerson != currentIM.ContactPerson)
                        Changes.Add(new Change
                        {
                            PropertyName = "Recipient",
                            Original = usernames[0],
                            Current = usernames[2]
                        });
                    if (originalIM.RequestedPerson != currentIM.RequestedPerson)
                        Changes.Add(new Change
                        {
                            PropertyName = "Contact",
                            Original = usernames[1],
                            Current = usernames[3]
                        });
                    if (originalIM.GroupId != currentIM.GroupId)
                        Changes.Add(new Change
                        {
                            Original = originalIM.Group.GroupName,
                            Current = currentIM.Group.GroupName,
                            PropertyName = "Assigment group"
                        });
                    if (originalIM.Assignee != currentIM.Assignee)
                        Changes.Add(new Change
                        {
                            Original = originalIM.Assignee,
                            Current = currentIM.Assignee,
                            PropertyName = "Assignee"
                        });
                    if (originalIM.ImpactId != currentIM.ImpactId)
                        Changes.Add(new Change
                        {
                            Original = originalIM.ImpactId.ToString(),
                            Current = currentIM.ImpactId.ToString(),
                            PropertyName = "Impact"
                        });
                    if (originalIM.UrgencyId != currentIM.UrgencyId)
                        Changes.Add(new Change
                        {
                            Original = originalIM.UrgencyId.ToString(),
                            Current = currentIM.UrgencyId.ToString(),
                            PropertyName = "Urgency"
                        });
                    if (originalIM.Topic != currentIM.Topic)
                        Changes.Add(new Change
                        {
                            Original = originalIM.Topic,
                            Current = currentIM.Topic,
                            PropertyName = "Topic"
                        });
                    if (originalIM.Description != currentIM.Description)
                        Changes.Add(new Change
                        {
                            Original = originalIM.Description,
                            Current = currentIM.Description,
                            PropertyName = "Description"
                        });
                    break;
                case "ServiceRequestProxy":
                    ServiceRequest originalC = (ServiceRequest)originalRequest;
                    ServiceRequest currentC = (ServiceRequest)currentRequest;
                    if (originalC.ContactPerson != currentC.ContactPerson)
                        Changes.Add(new Change
                        {
                            PropertyName = "Recipient",
                            Original = usernames[0],
                            Current = usernames[2]
                        });
                    if (originalC.RequestedPerson != currentC.RequestedPerson)
                        Changes.Add(new Change
                        {
                            PropertyName = "Contact",
                            Original = usernames[1],
                            Current = usernames[3]
                        });
                    if (originalC.GroupId != currentC.GroupId)
                        Changes.Add(new Change
                        {
                            Original = originalC.Group.GroupName,
                            Current = currentC.Group.GroupName,
                            PropertyName = "Assigment group"
                        });
                    if (originalC.Assignee != currentC.Assignee)
                        Changes.Add(new Change
                        {
                            Original = originalC.Assignee,
                            Current = currentC.Assignee,
                            PropertyName = "Assignee"
                        });
                    if (originalC.ImpactId != currentC.ImpactId)
                        Changes.Add(new Change
                        {
                            Original = originalC.ImpactId.ToString(),
                            Current = currentC.ImpactId.ToString(),
                            PropertyName = "Impact"
                        });
                    if (originalC.UrgencyId != currentC.UrgencyId)
                        Changes.Add(new Change
                        {
                            Original = originalC.UrgencyId.ToString(),
                            Current = currentC.UrgencyId.ToString(),
                            PropertyName = "Urgency"
                        });
                    if (originalC.Topic != currentC.Topic)
                        Changes.Add(new Change
                        {
                            Original = originalC.Topic,
                            Current = currentC.Topic,
                            PropertyName = "Topic"
                        });
                    if (originalC.Description != currentC.Description)
                        Changes.Add(new Change
                        {
                            Original = originalC.Description,
                            Current = currentC.Description,
                            PropertyName = "Description"
                        });
                    break;
                default:
                    return null;

            }
            return Changes;
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


        public string NotificationBuilder(List<Change> changes)
        {
            string notification="";
            foreach(Change c in changes)
            {
                notification = "-" + c.PropertyName + " was changed from " +
                    c.Original + " to " + c.Current + "\n";
            }
            return notification;
        }

        public class AdditionalUpdateInfo
        {
            public AdditionalUpdateInfo()
            {
                Phase = "Open";
            }
            public string Username { get; set; }
            public string Phase { get; set; }
            public string Notification { get; set; }
        }
        public class Change
        {
            public string Original { get; set; }
            public string Current { get; set; }
            public string PropertyName { get; set; }
        }
    }
}
