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
    public class RequestsDataService : IRequestService
    {
        private string _username;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private AppServiceDeskDbContext _dbContext;
        private Status NewStatus = new Status();

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
            await _dbContext.Applications.Where(a => a.Group == null).ToListAsync();

        public async Task<List<Incident>> GetAllIncidents() =>
            await _dbContext.Incidents.Where(i => i.Group == null).ToListAsync();

        public async Task<List<TaskRequest>> GetAllTasks() =>
            await _dbContext.Tasks.Where(t => t.Group == null).ToListAsync();

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
                .Concat(tasks.Select(t => t.ContactPerson).ToList())
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
            for (int i = 0; i < amount; i++)
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
                .Where(i => i.GroupId == groupId)
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

        public async Task<List<AssigmentGroup>> GetAllMemberingGroups(string activeUser)
        {
            List<AssigmentGroup> groups = new List<AssigmentGroup>();

            var groupsId = await _dbContext.Members
                .Where(i => i.Username == activeUser)
                .Select(g => g.GroupId)
                .ToListAsync();

            for (int i = 0; i < groupsId.Count; i++)
            {
                groups.Add(await _dbContext.AssigmentGroup
                    .Where(g => g.GroupId == groupsId[i])
                    .FirstOrDefaultAsync());
            }

            return groups;
        }

        public List<AssigmentGroup> GetAllMemberingGroupsNotAsync(string activeUser)
        {
            List<AssigmentGroup> groups = new List<AssigmentGroup>();

            var groupsId = _dbContext.Members
                .Where(i => i.Username == activeUser)
                .Select(g => g.GroupId)
                .ToList();

            for (int i = 0; i < groupsId.Count(); i++)
            {
                groups.Add(_dbContext.AssigmentGroup
                    .Where(g => g.GroupId == groupsId[i])
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
            .Where(d => d.Category.Designation == designation)
            .ToListAsync();

        public async Task<List<Priority>> GetPriorityByLevel() =>
            await _dbContext.Priorities.ToListAsync();

        public async Task<List<CloserDue>> GetClosureCodes() =>
            await _dbContext.CloserDues.ToListAsync();

        private bool CheckWaitingEnd(Status current, Status newStatus)
        {
            if(newStatus.State.StateName == "Waiting" && 
                current.State.StateName == "Open")
            {
                var today = DateTime.Now.ToUniversalTime();
                if(today < current.OpensAt)
                {
                    var diference = current.OpensAt - today;
                    newStatus.DueTime -= diference;
                    NewStatus = newStatus;

                    return true;
                }
            }
            return false;
        }
        public async Task UpdateC(ServiceRequest request, Data.Services.AdditionalUpdateInfo additional)
        {
            try
            {
                var curr = await _dbContext.Applications
                    .Where(i => i.RequestId == request.RequestId)
                    .Select(s=>s.History.ActiveStatus)
                    .FirstOrDefaultAsync();
                Status newStatus = await CreateStatus(DateTime.Now, request.History.ActiveStatus.DueTime, additional);
                if (CheckWaitingEnd(curr, request.History.ActiveStatus))
                    request.History.ActiveStatus = NewStatus;
                newStatus.CreatedBy = additional.Username;
                newStatus.HistoryId = request.HistoryId;
                newStatus.GroupId = request.Group.GroupId;
                await UpdateHistory(request.History.ChangeId, newStatus.StatusId);
                _dbContext = _databaseContextFactory.CreateServiceDeskDbContext();

                _dbContext.Update(request);
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception e)
            {

            }
        }
        public async Task UpdateT(TaskRequest task, Data.Services.AdditionalUpdateInfo additional)
        {
            var curr = await _dbContext.Applications
                .Where(i => i.RequestId == task.TaskId)
                .Select(s => s.History.ActiveStatus)
                .FirstOrDefaultAsync();
            Status newStatus = await CreateStatus(DateTime.Now, task.History.ActiveStatus.DueTime, additional);
            if (CheckWaitingEnd(curr, task.History.ActiveStatus))
                task.History.ActiveStatus = NewStatus;
            newStatus.CreatedBy = additional.Username;
            newStatus.HistoryId = task.HistoryId;
            newStatus.GroupId = task.Group.GroupId;
            await UpdateHistory(task.History.ChangeId, newStatus.StatusId);

            _dbContext.Update(task);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateIM(Incident incident, Data.Services.AdditionalUpdateInfo additional)
        {
            var curr = await _dbContext.Applications
                .Where(i => i.RequestId == incident.IncidentId)
                .Select(s => s.History.ActiveStatus)
                .FirstOrDefaultAsync();
            Status newStatus = await CreateStatus(DateTime.Now, incident.History.ActiveStatus.DueTime, additional);
            if (CheckWaitingEnd(curr, incident.History.ActiveStatus))
                incident.History.ActiveStatus = NewStatus;
            newStatus.CreatedBy = additional.Username;
            newStatus.HistoryId = incident.HistoryId;
            newStatus.GroupId = incident.Group.GroupId;
            await UpdateHistory(incident.History.ChangeId, newStatus.StatusId);

            _dbContext.Update(incident);
            await _dbContext.SaveChangesAsync();
        }
        private async Task UpdateHistory(int historyId, int statusId)
        {
            var history = await _dbContext.StatusHistory
                 .Where(i => i.ChangeId == historyId)
                 .FirstOrDefaultAsync();

            history.StatusId = statusId;

            await _dbContext.SaveChangesAsync();
        }
        private async Task<Status> CreateStatus(DateTime createTime, DateTime dueTime,
            Data.Services.AdditionalUpdateInfo additional)
        {
            Status status = new Status();
            status.CreateDate = createTime.ToUniversalTime();

            if (status.CreateDate > status.DueTime)
                status.Expired = true;
            else
                status.Expired = false;

            status.DueTime = dueTime;
            status.State = await _dbContext.States
                .Where(n => n.StateName == additional.Phase)
                .FirstOrDefaultAsync();
            status.Notification = additional.Notification;

            _dbContext.Statuses.Add(status);
            await _dbContext.SaveChangesAsync();

            return status;
        }
        public async Task<string> GetChangeAssignee(int requestId) =>
            await _dbContext.Applications
            .Where(i => i.RequestId == requestId)
            .Select(a => a.Assignee)
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

        public async Task<List<Incident>> GetOpenIncidentsList() =>
            await _dbContext.Incidents.Where(o => (o.History.ActiveStatus.State.StateName == "Open" ||
            o.History.ActiveStatus.State.StateName == "Waiting") && o.GroupId != null).ToListAsync();

        public async Task AddAssociatedIncident(Incident request, string username, Incident choosenIncident)
        {
            Data.Services.AdditionalUpdateInfo additional = new Data.Services.AdditionalUpdateInfo();
            additional.Username = username;
            request.IsAssociated = true;
            await UpdateIM(request, additional);
            choosenIncident.AffectedIncidents.Add(request);
            _dbContext.Entry(choosenIncident).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsGroupMember(string username, int id) =>
            await _dbContext.Members.AnyAsync(i => i.GroupId == id && i.Username == username);

        public async Task<List<string>> GetMembers(int groupId) =>
            await _dbContext.Members
            .Where(g => g.GroupId == groupId)
            .Select(u=>u.Username)
            .ToListAsync();

        public async Task InsertNewMessage(int historyId, string message, string username)
        {
            try
            {


            Status activeStatus = await _dbContext.StatusHistory
                .Where(i => i.StatusId == historyId)
                .Select(a=>a.ActiveStatus)
                .FirstOrDefaultAsync();

            Status status = new Status();
            status.StateId = activeStatus.StateId;
            status.CreateDate = DateTime.Now.ToUniversalTime();
            status.DueTime = activeStatus.DueTime;
            status.CreatedBy = username;
            status.HistoryId = historyId;
            status.Message = message;
            status.NotNotification = true;

            _dbContext.Statuses.Add(status);
            await _dbContext.SaveChangesAsync();
            }
            catch(Exception e)
            {

            }
        }
        public async Task<List<Incident>> GetIncidents() =>
            await _dbContext.Incidents.ToListAsync();
        public async Task<List<ServiceRequest>> GetChanges() =>
            await _dbContext.Applications.ToListAsync();

        public async Task<List<TaskRequest>> GetTasks() =>
            await _dbContext.Tasks.ToListAsync();

    }
}
