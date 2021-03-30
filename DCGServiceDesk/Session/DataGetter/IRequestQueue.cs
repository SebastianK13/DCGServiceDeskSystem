using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    public interface IRequestQueue
    {
        Task<List<object>> GetRequests();
        Task<List<Incident>> GetIncidents();
        Task<List<TaskRequest>> GetTasks();
        Task<List<ServiceRequest>> GetChanges();
        Task<bool> ChangeRequestAsignee(int requestId, string requestType, string username);
        Task<List<object>> GetGroupRequests(int groupId);
        List<AssigmentGroup> GetUserGroups(string activeUser);
        Task<List<AssigmentGroup>> GetGroups();
        Task<List<object>> GetAssignedRequests(string username);
        Task<List<State>> GetAllStates();
        void RefreshData();
        Task<TaskRequest> GetTask(int taskId);
        Task<ServiceRequest> GetChange(int requestId);
        Task<Incident> GetIncident(int incidentId);
        Task<List<Categorization>> GetSubcategories(string designation);
        Task<List<Urgency>> GetUrgencies();
        Task<List<Impact>> GetImpacts();
        Task<List<Priority>> GetPriority();
        Task<List<CloserDue>> GetCloserDues();
        Task UpdateServiceRequest(ServiceRequest request);
        Task UpdateTaskRequest(TaskRequest task);
        Task UpdateIncident(Incident incident);
        Task<string> GetChangeAssignee(int requestId);
        Task<string> GetTaskAssignee(int requestId);
        Task<string> GetIncidentAssignee(int requestId);
    }
}
