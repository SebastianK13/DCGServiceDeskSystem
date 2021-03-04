using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class RequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private HomeViewModel _hVM;
        private List<string> contactId = new List<string>();
        private List<string> requestedId = new List<string>();
        private List<string> ClassTypes = new List<string> { "IncidentProxy", "ServiceRequestProxy", "TaskRequestProxy" };

        public RequestCommand(IRequestQueue requestQueue, IUserInfo userInfo, IEmployeeProfile employeeProfile, HomeViewModel hVM)
        {
            _requestQueue = requestQueue;
            _userInfo = userInfo;
            _employeeProfile = employeeProfile;
            _hVM = hVM;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                string choosen = parameter as string;
                if (parameter.GetType().Name == "TabContainer")
                {
                    choosen = ((TabContainer)parameter).ServiceRequests.GetType().Name;
                }
                else if(parameter.GetType().Name == "AssigmentGroupProxy")
                {
                    choosen = "Group";
                }

                switch (choosen)
                {
                    case "RequestQueue":
                        var requests = await _requestQueue.GetRequests();
                        await SetAllRequestQueue(requests, "Service Requests");
                        break;
                    case "IncidentQueue":
                        await SetIncidentsQueue();
                        break;
                    case "ChangesQueue":
                        await SetChangesQueue();
                        break;
                    case "TasksQueue":
                        await SetTasksQueue();
                        break;
                    case "CurrentAssignRequests":
                        var assignedRequests = await _requestQueue.GetAssignedRequests(_hVM.loggedUser.ActiveUser);
                        await SetAllRequestQueue(assignedRequests, "Requests assigned");
                        break;
                    case "Group":
                        var groupRequests = await _requestQueue.GetGroupRequests(((AssigmentGroup)parameter).GroupId);
                        await SetAllRequestQueue(groupRequests, ((AssigmentGroup)parameter).GroupName);
                        break;
                    case "IncidentProxy":
                        EscalatedOrNot(parameter, "IM");
                        break;
                    case "ServiceRequestProxy":
                        EscalatedOrNot(parameter, "C");
                        break;
                    case "TaskRequestProxy":
                        EscalatedOrNot(parameter, "T");
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {

            }
        }
        private async Task EscalatedOrNot(object parameter, string requestType)
        {
            var states = await _requestQueue.GetAllStates();

            switch (requestType)
            {
                case "T":
                    var t = ExtractTask(parameter);
                    string labelT = SetRequestId(t.TaskId, "T");
                    if(t.Group == null)
                    {
                        _hVM.Tabs.Add(new RequestViewModel(t, labelT, ExtractAdditionalInfo(parameter), states));
                    }
                    else
                    {
                        _hVM.Tabs.Add(new EscalatedViewModel(t, labelT, ExtractAdditionalInfo(parameter),states));
                    }
                    break;
                case "IM":
                    var im = ExtractIncident(parameter);
                    string labelIM = SetRequestId(im.IncidentId, "IM");
                    if (im.Group == null)
                    {
                        _hVM.Tabs.Add(new RequestViewModel(im, labelIM, ExtractAdditionalInfo(parameter),states));
                    }
                    else
                    {
                        _hVM.Tabs.Add(new EscalatedViewModel(im, labelIM, ExtractAdditionalInfo(parameter),states));
                    }
                    break;
                case "C":
                    var c = ExtractChange(parameter);
                    string labelC = SetRequestId(c.RequestId, "C");
                    if (c.Group == null)
                    {
                        _hVM.Tabs.Add(new RequestViewModel(c, labelC, ExtractAdditionalInfo(parameter), states));
                    }
                    else
                    {
                        _hVM.Tabs.Add(new EscalatedViewModel(c, labelC, ExtractAdditionalInfo(parameter), states));
                    }
                    break;
            }
        }
        private Incident ExtractIncident(object parameter) =>
            (Incident)((TabContainer)parameter).ServiceRequests;

        private ServiceRequest ExtractChange(object parameter) =>
            (ServiceRequest)((TabContainer)parameter).ServiceRequests;
        private TaskRequest ExtractTask(object parameter) =>
            (TaskRequest)((TabContainer)parameter).ServiceRequests;

        private CommunicationInfo ExtractAdditionalInfo(object parameter) =>
            ((TabContainer)parameter).CommunicationInfo;

        public async Task SetAllRequestQueue(List<object> requests, string queueName)
        {
            List<int> contact = (List<int>)requests[1];
            List<int> requested = (List<int>)requests[2];
            List<string> contactId = await _employeeProfile.GetUserId(contact);
            List<string> requestedId = await _employeeProfile.GetUserId(requested);
            var aInfo = await _userInfo.GetUserName(contactId, requestedId);
            AddRequestIdsMixed(aInfo, (List<int>)requests[4], (List<string>)requests[3]);
            _hVM.SetRequests((List<object>)requests[0], aInfo,
            (List<string>)requests[3], queueName);
        }

        private async Task SetIncidentsQueue()
        {
            var incidents = await _requestQueue.GetIncidents();
            List<int> contact = incidents.Select(i => i.ContactPerson).ToList();
            List<int> requested = incidents.Select(i => i.RequestedPerson).ToList();
            List<string> contactId2 = await _employeeProfile.GetUserId(contact);
            List<string> requestedId2 = await _employeeProfile.GetUserId(requested);
            var tInfo = await _userInfo.GetUserName(contactId, requestedId);
            AddRequestIds(tInfo, incidents.Select(i => i.IncidentId).ToList(), "IM");
            _hVM.SetIncidents(incidents, tInfo);
        }

        private async Task SetChangesQueue()
        {
            var changes = await _requestQueue.GetChanges();
            List<int> contactC = changes.Select(i => i.ContactPerson).ToList();
            List<int> requestedC = changes.Select(i => i.RequestedPerson).ToList();
            List<string> contactIdC = await _employeeProfile.GetUserId(contactC);
            List<string> requestedIdC = await _employeeProfile.GetUserId(requestedC);
            var cInfo = await _userInfo.GetUserName(contactIdC, requestedIdC);
            AddRequestIds(cInfo, changes.Select(i => i.RequestId).ToList(), "C");
            _hVM.SetChanges(changes, cInfo);
        }

        private async Task SetTasksQueue()
        {
            var tasks = await _requestQueue.GetTasks();
            List<int> contactT = tasks.Select(i => i.ContactPerson).ToList();
            List<int> requestedT = tasks.Select(i => i.RequestedPerson).ToList();
            List<string> contactIdT = await _employeeProfile.GetUserId(contactT);
            List<string> requestedIdT = await _employeeProfile.GetUserId(requestedT);
            var info = await _userInfo.GetUserName(contactIdT, requestedIdT);
            AddRequestIds(info, tasks.Select(i => i.TaskId).ToList(), "T");
            _hVM.SetTasks(tasks, info);
        }

        private List<CommunicationInfo> AddRequestIds(List<CommunicationInfo> info, List<int> Ids, string requestType)
        {
            for(int i = 0; i < info.Count; i++)
            {
                int temp = 1000000 + Ids[i];
                info[i].RequestId = requestType + temp.ToString().Substring(1);
            }

            return info;
        }
        private string SetRequestId(int id, string requestType)
        {
            int temp = 1000000 + id;
            return requestType + temp.ToString().Substring(1);
        }
        private List<CommunicationInfo> AddRequestIdsMixed(List<CommunicationInfo> info, List<int> Ids, List<string> requestTypes)
        {
            for (int i = 0; i < info.Count; i++)
            {
                int temp = 1000000 + Ids[i];
                info[i].RequestId = SetShortcut(requestTypes[i]) + temp.ToString().Substring(1);
            }

            return info;
        }
        private string SetShortcut(string typeName)
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
    }
}
