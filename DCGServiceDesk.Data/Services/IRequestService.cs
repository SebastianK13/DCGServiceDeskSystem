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
        void RefreshDatabase();
        Task<TaskRequest> GetSingleTask(int taskId);
        Task<ServiceRequest> GetSingleChange(int requestId);
        Task<Incident> GetSingleIncident(int incidentId);
        Task<List<AssigmentGroup>> GetAllGroups();
        Task<List<Impact>> GetAllImpacts();
        Task<List<Urgency>> GetAllUrgencies();
        Task<List<Categorization>> GetAllSubcategories(string designation);
        Task<List<Priority>> GetPriorityByLevel();
        Task<List<CloserDue>> GetClosureCodes();
        Task UpdateC(ServiceRequest request);
        Task UpdateT(TaskRequest task);
        Task UpdateIM(Incident incident);
        Task<string> GetChangeAssignee(int requestId);
        Task<string> GetTaskAssignee(int requestId);
        Task<string> GetIncidentAssignee(int requestId);
    }
}
