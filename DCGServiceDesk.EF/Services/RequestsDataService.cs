using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.EF.Context;
using DCGServiceDesk.EF.Factory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.EF.Services
{
    public class RequestsDataService:IRequestService
    {
        private string _username;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private AppServiceDeskDbContext _dbContext;

        public RequestsDataService(IDatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
            _dbContext = _databaseContextFactory.CreateServiceDeskDbContext();
        }
        public void RefreshDatabase() =>
            _dbContext = _databaseContextFactory.CreateServiceDeskDbContext();

        public Task<ServiceRequest> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ServiceRequest>> GetAll() =>
            await _dbContext.Applications.Where(a=>a.Group == null).ToListAsync();

        public async Task<List<Incident>> GetAllIncidents() => 
            await _dbContext.Incidents.Where(i=>i.Group == null).ToListAsync();

        public async Task<List<TaskRequest>> GetAllTasks() => 
            await _dbContext.Tasks.Where(t=>t.Group == null).ToListAsync();

        public async Task<List<object>> GetAllRequests()
        {
            List<int> requestIds = new List<int>();
            List<object> requests = new List<object>();
            var changes = await GetAll();
            var incidents = await GetAllIncidents();
            var tasks = await GetAllTasks();

            requests = requests.Concat(changes).Concat(incidents).Concat(tasks).ToList();

            List<int> contacts = (List<int>)(changes.Select(c => c.ContactPerson).ToList())
                .Concat(incidents.Select(i => i.ContactPerson).ToList())
                .Concat(tasks.Select(t=>t.ContactPerson).ToList())
                .ToList();

            List<int> requested = (List<int>)(changes.Select(c => c.RequestedPerson).ToList())
                .Concat(incidents.Select(i => i.RequestedPerson).ToList())
                .Concat(tasks.Select(t => t.RequestedPerson).ToList())
                .ToList();

            requestIds = requestIds
                .Concat(changes.Select(i => i.RequestId))
                .Concat(incidents.Select(i => i.IncidentId))
                .Concat(tasks.Select(i => i.TaskId))
                .ToList();

            List<string> typeList = new List<string>();
            typeList = CreateTypeList("Changes", changes.Count, typeList);
            typeList = CreateTypeList("Incidents", incidents.Count, typeList);
            typeList = CreateTypeList("Tasks", tasks.Count, typeList);

            return new List<object> { requests, contacts, requested, typeList, requestIds };
        }
        public List<string> CreateTypeList(string type, int amount, List<string> typeList)
        {
            for(int i = 0; i < amount; i++)
            {
                typeList.Add(type);
            }
            return typeList;
        }
        public async Task<bool> UpdateRequestAssignee(int requestId, string requestType, string username)
        {
            _username = username;
            switch (requestType)
            {
                case "Incident":
                    return await UpdateIncidentAssignee(requestId);
                case "Change":
                    return await UpdateChangeAssignee(requestId);
                case "Task":
                    return await UpdateTaskAssignee(requestId);
            }

            return false;
        }

        public async Task<bool> UpdateIncidentAssignee(int id)
        {
            Incident im = await _dbContext.Incidents
                .Where(i => i.IncidentId == id && i.Assignee == null)
                .FirstOrDefaultAsync();

            int gId = await _dbContext.AssigmentGroup
                .Where(n => n.GroupName == "Service Desk")
                .Select(i => i.GroupId)
                .FirstOrDefaultAsync();

            if (im != null)
            {
                im.GroupId = gId;
                im.Assignee = _username;

                return await _dbContext.SaveChangesAsync() > 0 ? true : false;
            }
            return false;
        }
        public async Task<bool> UpdateChangeAssignee(int id)
        {
            ServiceRequest c = await _dbContext.Applications
                .Where(i => i.RequestId == id && i.Assignee == null)
                .FirstOrDefaultAsync();

            int gId = await _dbContext.AssigmentGroup
                .Where(n => n.GroupName == "Service Desk")
                .Select(i => i.GroupId)
                .FirstOrDefaultAsync();

            if (c != null)
            {
                c.GroupId = gId;
                c.Assignee = _username;

                return await _dbContext.SaveChangesAsync() > 0 ? true : false;
            }
            return false;
        }
        public async Task<bool> UpdateTaskAssignee(int id)
        {
            TaskRequest t = await _dbContext.Tasks
                .Where(i => i.TaskId == id && i.Assignee == null)
                .FirstOrDefaultAsync();

            int gId = await _dbContext.AssigmentGroup
                .Where(n => n.GroupName == "Service Desk")
                .Select(i => i.GroupId)
                .FirstOrDefaultAsync();
            if (t != null)
            {
                t.GroupId = gId;
                t.Assignee = _username;

                return await _dbContext.SaveChangesAsync() > 0 ? true : false;
            }
            return false;
        }

        public async Task<List<object>> GetRequestsFromGroup(int groupId)
        {
            List<int> requestIds = new List<int>();
            List<object> requests = new List<object>();
            var changes = await _dbContext.Applications
                .Where(i=>i.GroupId == groupId)
                .ToListAsync();

            var incidents = await _dbContext.Incidents
                .Where(i => i.GroupId == groupId)
                .ToListAsync();

            var tasks = await _dbContext.Tasks
                .Where(i => i.GroupId == groupId)
                .ToListAsync();

            requests = requests.Concat(changes).Concat(incidents).Concat(tasks).ToList();

            List<int> contacts = (List<int>)(changes.Select(c => c.ContactPerson).ToList())
                .Concat(incidents.Select(i => i.ContactPerson).ToList())
                .Concat(tasks.Select(t => t.ContactPerson).ToList())
                .ToList();

            List<int> requested = (List<int>)(changes.Select(c => c.RequestedPerson).ToList())
                .Concat(incidents.Select(i => i.RequestedPerson).ToList())
                .Concat(tasks.Select(t => t.RequestedPerson).ToList())
                .ToList();

            List<string> typeList = new List<string>();
            typeList = CreateTypeList("Changes", changes.Count, typeList);
            typeList = CreateTypeList("Incidents", incidents.Count, typeList);
            typeList = CreateTypeList("Tasks", tasks.Count, typeList);

            requestIds = requestIds
                .Concat(changes.Select(i => i.RequestId))
                .Concat(incidents.Select(i => i.IncidentId))
                .Concat(tasks.Select(i => i.TaskId))
                .ToList();

            return new List<object> { requests, contacts, requested, typeList, requestIds };
        }

        public List<AssigmentGroup> GetAllMemberingGroups(string activeUser)
        {
            List<AssigmentGroup> groups = new List<AssigmentGroup>();

            var groupsId = _dbContext.Members
                .Where(i => i.Username == activeUser)
                .Select(g => g.GroupId)
                .ToList();

            for (int i = 0; i < groupsId.Count(); i++)
            {
                groups.Add( _dbContext.AssigmentGroup
                    .Where(g=>g.GroupId == groupsId[i])
                    .FirstOrDefault());
            }

            return groups;
        }

        public async Task<List<object>> GetAssignedNotEscalated(string username)
        {
            List<int> requestIds = new List<int>();
            List<object> requests = new List<object>();
            var changes = await _dbContext.Applications
                .Where(i => i.Group.GroupName == "Service Desk" &&
                i.History.ActiveStatus.StateId == 1 && 
                i.Assignee == username)
                .ToListAsync();

            var incidents = await _dbContext.Incidents
                .Where(i => i.Group.GroupName == "Service Desk" &&
                i.History.ActiveStatus.StateId == 1 &&
                i.Assignee == username)
                .ToListAsync();

            var tasks = await _dbContext.Tasks
                .Where(i => i.Group.GroupName == "Service Desk" &&
                i.History.ActiveStatus.StateId == 1 &&
                i.Assignee == username)
                .ToListAsync();

            requests = requests.Concat(changes).Concat(incidents).Concat(tasks).ToList();

            List<int> contacts = (List<int>)(changes.Select(c => c.ContactPerson).ToList())
                .Concat(incidents.Select(i => i.ContactPerson).ToList())
                .Concat(tasks.Select(t => t.ContactPerson).ToList())
                .ToList();

            List<int> requested = (List<int>)(changes.Select(c => c.RequestedPerson).ToList())
                .Concat(incidents.Select(i => i.RequestedPerson).ToList())
                .Concat(tasks.Select(t => t.RequestedPerson).ToList())
                .ToList();

            List<string> typeList = new List<string>();
            typeList = CreateTypeList("Changes", changes.Count, typeList);
            typeList = CreateTypeList("Incidents", incidents.Count, typeList);
            typeList = CreateTypeList("Tasks", tasks.Count, typeList);

            requestIds = requestIds
                .Concat(changes.Select(i => i.RequestId))
                .Concat(incidents.Select(i => i.IncidentId))
                .Concat(tasks.Select(i => i.TaskId))
                .ToList();

            return new List<object> { requests, contacts, requested, typeList, requestIds };
        }

        public async Task<List<State>> GetRequestStates() =>
            await _dbContext.States.ToListAsync();

        public async Task<TaskRequest> GetSingleTask(int taskId) =>
            await _dbContext.Tasks.Where(i => i.TaskId == taskId).FirstOrDefaultAsync();

        public async Task<ServiceRequest> GetSingleChange(int requestId) =>
            await _dbContext.Applications.Where(i => i.RequestId == requestId).FirstOrDefaultAsync();

        public async Task<Incident> GetSingleIncident(int incidentId) =>
            await _dbContext.Incidents.Where(i => i.IncidentId == incidentId).FirstOrDefaultAsync();

        public async Task<List<AssigmentGroup>> GetAllGroups() =>
            await _dbContext.AssigmentGroup.ToListAsync();

        public async Task<List<Impact>> GetAllImpacts() =>
            await _dbContext.Impacts.ToListAsync();

        public async Task<List<Urgency>> GetAllUrgencies() =>
            await _dbContext.Urgencies.ToListAsync();

        public async Task<List<Categorization>> GetAllSubcategories(string designation) =>
            await _dbContext.Categorizations
            .Where(d=>d.Category.Designation == designation)
            .ToListAsync();

        public async Task<List<Priority>> GetPriorityByLevel() =>
            await _dbContext.Priorities.ToListAsync();

        public async Task<List<CloserDue>> GetClosureCodes() =>
            await _dbContext.CloserDues.ToListAsync();

        public async Task UpdateC(ServiceRequest request)
        {
            ServiceRequest c = await _dbContext.Applications
                .Where(i => i.RequestId == request.RequestId)
                .FirstOrDefaultAsync();

            c = request;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateT(TaskRequest task)
        {
            TaskRequest t2 = await _dbContext.Tasks.Where(i => i.TaskId == 5).FirstOrDefaultAsync();
            TaskRequest t = await _dbContext.Tasks
                .Where(i => i.TaskId == task.TaskId)
                .FirstOrDefaultAsync();

            Status newStatus = await CreateStatus(DateTime.Now, task.History.ActiveStatus.DueTime);

            t.History.Status.Add(newStatus);
            t.History.ActiveStatus = task.History.ActiveStatus;
            t.History.CloserDue = task.History.CloserDue;
            t.History.Solution = task.History.Solution;
            t.RequestedPerson = task.RequestedPerson;
            t.ContactPerson = task.ContactPerson;
            t.Category = task.Category;
            t.Impact = task.Impact;
            t.Urgency = task.Urgency;
            t.Priority = task.Priority;
            t.Topic = task.Topic;
            t.Description = task.Description;

            await _dbContext.SaveChangesAsync();
        }
        private async Task<Status> CreateStatus(DateTime createTime, DateTime dueTime)
        {
            Status status = new Status();
            status.StateId = await _dbContext.States
                .Where(n => n.StateName == "New")
                .Select(i => i.StateId)
                .FirstOrDefaultAsync();
            status.CreateDate = createTime.ToUniversalTime();

            if (status.CreateDate > status.DueTime)
                status.Expired = true;
            else
                status.Expired = false;

            status.DueTime = dueTime;
            status.State = await _dbContext.States
                .Where(n => n.StateName == "Closed")
                .FirstOrDefaultAsync();

            _dbContext.Statuses.Add(status);
            await _dbContext.SaveChangesAsync();

            return status;
        }
        public async Task UpdateIM(Incident incident)
        {
            Incident im = await _dbContext.Incidents
                .Where(i => i.IncidentId == incident.IncidentId)
                .FirstOrDefaultAsync();

            im = incident;

            await _dbContext.SaveChangesAsync();
        }
        public async Task<string> GetChangeAssignee(int requestId) =>
            await _dbContext.Applications
            .Where(i => i.RequestId == requestId)
            .Select(a=>a.Assignee)
            .FirstOrDefaultAsync();

        public async Task<string> GetTaskAssignee(int requestId) =>
            await _dbContext.Tasks
            .Where(i => i.TaskId == requestId)
            .Select(a => a.Assignee)
            .FirstOrDefaultAsync();

        public async Task<string> GetIncidentAssignee(int requestId) =>
            await _dbContext.Incidents
            .Where(i => i.IncidentId == requestId)
            .Select(a => a.Assignee)
            .FirstOrDefaultAsync();
        public Task<bool> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(int id, User entity)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceRequest> Update(int id, ServiceRequest entity)
        {
            throw new NotImplementedException();
        }

    }
}
