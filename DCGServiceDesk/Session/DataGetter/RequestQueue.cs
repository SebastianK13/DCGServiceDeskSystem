using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;

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

        public List<AssigmentGroup> GetGroups(string activeUser) =>
             _requestService.GetAllMemberingGroups(activeUser);

        public async Task<List<object>> GetAssignedRequests(string username) =>
            await _requestService.GetAssignedNotEscalated(username);
    }
}
