using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.ViewModels;

namespace DCGServiceDesk.Session.DataGetter
{
    class RequestQueue : IRequestQueue
    {
        private readonly IRequestService _requestService;
        public RequestQueue(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<List<Incident>> GetIncidents() =>
            await _requestService.GetAllIncidents();

        public async Task<List<ServiceRequest>> GetChanges() =>
            await _requestService.GetAll();

        public async Task<List<TaskRequest>> GetTasks() =>
            await _requestService.GetAllTasks();

        public async Task<List<object>> GetRequests() =>
            await _requestService.GetAllRequests();

        public async Task<bool> ChangeRequestAsignee(int requestId, string requestType, string username) =>
            await _requestService.UpdateRequestAssignee(requestId, requestType, username);

        public async Task<List<object>> GetGroupRequests(int groupId) =>
            await _requestService.GetRequestsFromGroup(groupId);

        public async Task<List<object>> GetAssignedRequests(string username) =>
            await _requestService.GetAssignedNotEscalated(username);

        public async Task<List<State>> GetAllStates() =>
            await _requestService.GetRequestStates();

        public void RefreshData() =>
            _requestService.RefreshDatabase();

        public async Task<TaskRequest> GetTask(int taskId) =>
            await _requestService.GetSingleTask(taskId);

        public async Task<ServiceRequest> GetChange(int requestId) =>
            await _requestService.GetSingleChange(requestId);

        public async Task<Incident> GetIncident(int incidentId) =>
            await _requestService.GetSingleIncident(incidentId);

        public async Task<List<AssigmentGroup>> GetUserGroups(string activeUser) =>
             await _requestService.GetAllMemberingGroups(activeUser);

        public List<AssigmentGroup> GetUserGroupsNotAsync(string activeUser) =>
             _requestService.GetAllMemberingGroupsNotAsync(activeUser);

        public async Task<List<AssigmentGroup>> GetGroups() =>
            await _requestService.GetAllGroups();

        public async Task<List<Categorization>> GetSubcategories(string designation) =>
            await _requestService.GetAllSubcategories(designation);

        public async Task<List<Urgency>> GetUrgencies() =>
            await _requestService.GetAllUrgencies();

        public async Task<List<Impact>> GetImpacts() =>
            await _requestService.GetAllImpacts();

        public async Task<List<Priority>> GetPriority() =>
            await _requestService.GetPriorityByLevel();

        public async Task<List<CloserDue>> GetCloserDues() =>
            await _requestService.GetClosureCodes();

        public async Task UpdateServiceRequest(ServiceRequest request, string username, string stateName = "Open") =>
            await _requestService.UpdateC(request, username,stateName);

        public async Task UpdateTaskRequest(TaskRequest task, string username, string stateName = "Open") =>
            await _requestService.UpdateT(task, username, stateName);

        public async Task UpdateIncident(Incident incident, string username, string stateName="Open") =>
            await _requestService.UpdateIM(incident, username, stateName);

        public async Task<string> GetChangeAssignee(int requestId) =>
            await _requestService.GetChangeAssignee(requestId);

        public async Task<string> GetTaskAssignee(int requestId) =>
            await _requestService.GetTaskAssignee(requestId);

        public async Task<string> GetIncidentAssignee(int requestId) =>
            await _requestService.GetIncidentAssignee(requestId);

        public async Task<List<Incident>> GetOpenIncidents() =>
            await _requestService.GetOpenIncidentsList();

        public async Task AddAssociatedIM(Incident request, string username, OpenIncidentContainer choosenIncident) =>
            await _requestService.AddAssociatedIncident(request, username, choosenIncident.Incident);

        public async Task<bool> IsGroupMember(string username, int id) =>
            await _requestService.IsGroupMember(username, id);
    }
}
