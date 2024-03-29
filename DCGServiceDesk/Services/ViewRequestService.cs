﻿using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Services
{
    public class ViewRequestService: IViewRequestService
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private readonly DbInterfaceContainer _interfaceContainer;

        public ViewRequestService(DbInterfaceContainer interfaceContainer)
        {
            _interfaceContainer = interfaceContainer;
            _requestQueue = _interfaceContainer.RequestQueue;
            _userInfo = _interfaceContainer.UserInfo;
            _employeeProfile = _interfaceContainer.EmployeeProfile;
        }

        public async Task<RequestInfo> SetAllRequestQueue(List<object> requests, string queueName, bool isGroup = false)
        {
            string queueType;
            if (isGroup)
                queueType = "G";
            else
            {
                switch (queueName)
                {
                    case "Service Requests":
                        queueType = "SD";
                        break;
                    case "Requests assigned":
                        queueType = "AR";
                        break;
                    default:
                        queueType = "SD";
                        break;
                }
            }

            List<int> contact = (List<int>)requests[1];
            List<int> requested = (List<int>)requests[2];
            List<string> contactId = await _employeeProfile.GetUserId(contact);
            List<string> requestedId = await _employeeProfile.GetUserId(requested);
            var aInfo = await _userInfo.GetUserName(contactId, requestedId);
            RequestService.AddRequestIdsMixed(aInfo, (List<int>)requests[4], (List<string>)requests[3]);
            RequestInfo requestInfo = new RequestInfo 
            { 
                Requests = (List<object>)requests[0],
                Infos = aInfo,
                RequestTypes = (List<string>)requests[3],
                QueueName = queueName,
                QueueType = queueType
            };
            if (requestInfo.Requests.Count() == 0)
                requestInfo.Requests.Add(new Incident());
            return requestInfo;
        }

        public async Task<RequestInfo> SetIncidentQueue()
        {
            var incidents = await _requestQueue.GetIncidents();
            List<int> contact = incidents.Select(i => i.ContactPerson).ToList();
            List<int> requested = incidents.Select(i => i.RequestedPerson).ToList();
            List<string> contactIdI = await _employeeProfile.GetUserId(contact);
            List<string> requestedIdI = await _employeeProfile.GetUserId(requested);
            var tInfo = await _userInfo.GetUserName(contactIdI, requestedIdI);
            RequestService.AddRequestIds(tInfo, incidents.Select(i => i.IncidentId).ToList(), "IM");
            RequestInfo requestInfo = new RequestInfo
            {
                Requests = RequestService.ConvertTotListObject(incidents),
                Infos = tInfo,
                RequestTypes = new List<string> { "Incidents" },
                QueueName = "Incidents",
                QueueType = "IM"
            };
            if (requestInfo.Requests.Count() == 0)
                requestInfo.Requests.Add(new Incident());
            return requestInfo;
        }

        public async Task<RequestInfo> SetChangesQueue()
        {
            var changes = await _requestQueue.GetChanges();
            List<int> contactC = changes.Select(i => i.ContactPerson).ToList();
            List<int> requestedC = changes.Select(i => i.RequestedPerson).ToList();
            List<string> contactIdC = await _employeeProfile.GetUserId(contactC);
            List<string> requestedIdC = await _employeeProfile.GetUserId(requestedC);
            var cInfo = await _userInfo.GetUserName(contactIdC, requestedIdC);
            RequestService.AddRequestIds(cInfo, changes.Select(i => i.RequestId).ToList(), "C");
            RequestInfo requestInfo = new RequestInfo
            {
                Requests = RequestService.ConvertTotListObject(changes),
                Infos = cInfo,
                RequestTypes = new List<string> { "Changes" },
                QueueName = "Changes",
                QueueType = "C"
            };
            if (requestInfo.Requests.Count() == 0)
                requestInfo.Requests.Add(new ServiceRequest());
            return requestInfo;
        }

        public async Task<RequestInfo> SetTasksQueue()
        {
            var tasks = await _requestQueue.GetTasks();
            List<int> contactT = tasks.Select(i => i.ContactPerson).ToList();
            List<int> requestedT = tasks.Select(i => i.RequestedPerson).ToList();
            List<string> contactIdT = await _employeeProfile.GetUserId(contactT);
            List<string> requestedIdT = await _employeeProfile.GetUserId(requestedT);
            var info = await _userInfo.GetUserName(contactIdT, requestedIdT);
            RequestService.AddRequestIds(info, tasks.Select(i => i.TaskId).ToList(), "T");
            RequestInfo requestInfo = new RequestInfo
            {
                Requests = RequestService.ConvertTotListObject(tasks),
                Infos = info,
                RequestTypes = new List<string> { "Tasks" },
                QueueName = "Tasks",
                QueueType = "T"
            };
            if (requestInfo.Requests.Count() == 0)
                requestInfo.Requests.Add(new TaskRequest());
            return requestInfo;
        }

        public async Task<RequestInfo> SetAllIncidentsQueue(int id = 0, bool isSingle = false)
        {
            List<Incident> incidents = new List<Incident>();
            if(!isSingle)
                incidents = await _requestQueue.GetIncidentsIncludeClosed();
            else
            {
                var im = await _requestQueue.GetIncident(id);
                if(im != null)
                    incidents.Add(im);
            }
                
            if(incidents.Count > 0)
            {
                List<int> contact = incidents.Select(i => i.ContactPerson).ToList();
                List<int> requested = incidents.Select(i => i.RequestedPerson).ToList();
                List<string> contactIdI = await _employeeProfile.GetUserId(contact);
                List<string> requestedIdI = await _employeeProfile.GetUserId(requested);
                var tInfo = await _userInfo.GetUserName(contactIdI, requestedIdI);
                RequestService.AddRequestIds(tInfo, incidents.Select(i => i.IncidentId).ToList(), "IM");
                RequestInfo requestInfo = new RequestInfo
                {
                    Requests = RequestService.ConvertTotListObject(incidents),
                    Infos = tInfo,
                    RequestTypes = new List<string> { "Incidents" },
                    QueueName = "Incidents",
                    QueueType = "IM"
                };
                return requestInfo;
            }
            return null;
        }

        public async Task<RequestInfo> SetAllChangesQueue(int id = 0, bool isSingle = false)
        {
            List<ServiceRequest> changes = new List<ServiceRequest>();
            if(!isSingle)
                changes = await _requestQueue.GetChangesIncludeClosed();
            else
            {
                var c = await _requestQueue.GetChange(id);
                if (c != null)
                    changes.Add(c);
            }

            if (changes.Count > 0)
            {
                List<int> contactC = changes.Select(i => i.ContactPerson).ToList();
                List<int> requestedC = changes.Select(i => i.RequestedPerson).ToList();
                List<string> contactIdC = await _employeeProfile.GetUserId(contactC);
                List<string> requestedIdC = await _employeeProfile.GetUserId(requestedC);
                var cInfo = await _userInfo.GetUserName(contactIdC, requestedIdC);
                RequestService.AddRequestIds(cInfo, changes.Select(i => i.RequestId).ToList(), "C");
                RequestInfo requestInfo = new RequestInfo
                {
                    Requests = RequestService.ConvertTotListObject(changes),
                    Infos = cInfo,
                    RequestTypes = new List<string> { "Changes" },
                    QueueName = "Changes",
                    QueueType = "C"
                };
                return requestInfo;
            }
            return null;

        }

        public async Task<RequestInfo> SetAllTasksQueue(int id = 0, bool isSingle = false)
        {
            List<TaskRequest> tasks = new List<TaskRequest>();
            if(!isSingle)
                tasks = await _requestQueue.GetTasksIncludeClosed();
            else
            {
                var t = await _requestQueue.GetTask(id);
                if (t != null)
                    tasks.Add(t);
            }

            if(tasks.Count > 0)
            {
                List<int> contactT = tasks.Select(i => i.ContactPerson).ToList();
                List<int> requestedT = tasks.Select(i => i.RequestedPerson).ToList();
                List<string> contactIdT = await _employeeProfile.GetUserId(contactT);
                List<string> requestedIdT = await _employeeProfile.GetUserId(requestedT);
                var info = await _userInfo.GetUserName(contactIdT, requestedIdT);
                RequestService.AddRequestIds(info, tasks.Select(i => i.TaskId).ToList(), "T");
                RequestInfo requestInfo = new RequestInfo
                {
                    Requests = RequestService.ConvertTotListObject(tasks),
                    Infos = info,
                    RequestTypes = new List<string> { "Tasks" },
                    QueueName = "Tasks",
                    QueueType = "T"
                };
                return requestInfo;
            }
            return null;

        }
    }
    public class DbInterfaceContainer
    {
        public DbInterfaceContainer()
        {
        }

        public DbInterfaceContainer(IRequestQueue requestQueue, IUserInfo userInfo, IEmployeeProfile employeeProfile)
        {
            RequestQueue = requestQueue;
            UserInfo = userInfo;
            EmployeeProfile = employeeProfile;
        }

        public IRequestQueue RequestQueue { get; set; }
        public IUserInfo UserInfo { get; set; }
        public IEmployeeProfile EmployeeProfile { get; set; }
    }
}
