using DCGServiceDesk.Data.Models;
using DCGServiceDesk.ViewModels;
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
        Task<List<AssigmentGroup>> GetUserGroups(string activeUser);
        List<AssigmentGroup> GetUserGroupsNotAsync(string activeUser);
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
        Task UpdateServiceRequest(ServiceRequest request, string username, string stateName = "Open");
        Task UpdateTaskRequest(TaskRequest task, string username, string stateName = "Open");
        Task UpdateIncident(Incident incident, string username, string stateName="Open");
        Task<string> GetChangeAssignee(int requestId);
        Task<string> GetTaskAssignee(int requestId);
        Task<string> GetIncidentAssignee(int requestId);
        Task<List<Incident>> GetOpenIncidents();
        Task AddAssociatedIM(Incident request, string username, OpenIncidentContainer choosenIncident);
        Task<bool> IsGroupMember(string username, int id);
        Task<List<string>> GetMembers(int groupId);
        Task AddNewMessage(int historyId, string message, string username);
        Task UpdateTForOpenStatus(TaskRequest task, string adminUsername, string statusName);
        Task UpdateIMForOpenStatus(Incident incident, string adminUsername, string statusName);
        Task UpdateCForOpenStatus(ServiceRequest change, string adminUsername, string statusName);
    }
}
