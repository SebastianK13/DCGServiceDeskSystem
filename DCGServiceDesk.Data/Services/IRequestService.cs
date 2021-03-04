using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Services
{
    public interface IRequestService: ICrud<ServiceRequest>
    {
        Task<List<ServiceRequest>> GetAll();
        Task<List<object>> GetAllRequests();
        Task<List<Incident>> GetAllIncidents();
        Task<List<TaskRequest>> GetAllTasks();
        Task<bool> UpdateRequestAssignee(int requestId, string requestType, string username);
        Task<List<object>> GetRequestsFromGroup(int groupId);
        List<AssigmentGroup> GetAllMemberingGroups(string activeUser);
        Task<List<object>> GetAssignedNotEscalated(string username);
        Task<List<State>> GetRequestStates();
    }
}
