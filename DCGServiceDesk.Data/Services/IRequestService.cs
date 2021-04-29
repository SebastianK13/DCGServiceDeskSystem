using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Services
{
    public interface IRequestService
    {
        Task<List<ServiceRequest>> GetAll();
        Task<List<object>> GetAllRequests();
        Task<List<Incident>> GetAllIncidents();
        Task<List<TaskRequest>> GetAllTasks();
        Task<bool> UpdateRequestAssignee(int requestId, string requestType, string username);
        Task<List<object>> GetRequestsFromGroup(int groupId);
        Task<List<AssigmentGroup>> GetAllMemberingGroups(string activeUser);
        List<AssigmentGroup> GetAllMemberingGroupsNotAsync(string activeUser);
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
        Task UpdateC(ServiceRequest request, string username, string stateName = "Open");
        Task UpdateT(TaskRequest task, string username, string stateName = "Open");
        Task UpdateIM(Incident incident, string username, string stateName="Open");
        Task<string> GetChangeAssignee(int requestId);
        Task<string> GetTaskAssignee(int requestId);
        Task<string> GetIncidentAssignee(int requestId);
        Task<List<Incident>> GetOpenIncidentsList();
        Task AddAssociatedIncident(Incident request, string username, Incident choosenIncident);
        Task<bool> IsGroupMember(string username, int id);
        Task<List<string>> GetMembers(int groupId);
        Task InsertNewMessage(int historyId, string message, string username);
        Task UpdateTForOpenStatus(TaskRequest task, string adminUsername, string statusName);
        Task UpdateCForOpenStatus(ServiceRequest change, string adminUsername, string statusName);
        Task UpdateIMForOpenStatus(Incident incident, string adminUsername, string statusName);
    }
}
