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
        public Task<ServiceRequest> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ServiceRequest>> GetAll() =>
            await _dbContext.Applications.ToListAsync();

        public async Task<List<Incident>> GetAllIncidents() => 
            await _dbContext.Incidents.ToListAsync();

        public async Task<List<TaskRequest>> GetAllTasks() => 
            await _dbContext.Tasks.ToListAsync();

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
                .Where(i => i.IncidentId == id)
                .FirstOrDefaultAsync();

            int gId = await _dbContext.AssigmentGroup
                .Where(n => n.GroupName == "Service Desk")
                .Select(i => i.GroupId)
                .FirstOrDefaultAsync();

            im.GroupId = gId;
            im.Assignee = _username;

            return await _dbContext.SaveChangesAsync() > 0? true:false;
        }
        public async Task<bool> UpdateChangeAssignee(int id)
        {
            ServiceRequest c = await _dbContext.Applications
                .Where(i => i.RequestId == id)
                .FirstOrDefaultAsync();

            int gId = await _dbContext.AssigmentGroup
                .Where(n => n.GroupName == "Service Desk")
                .Select(i => i.GroupId)
                .FirstOrDefaultAsync();

            c.GroupId = gId;
            c.Assignee = _username;

            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task<bool> UpdateTaskAssignee(int id)
        {
            TaskRequest t = await _dbContext.Tasks
                .Where(i => i.TaskId == id)
                .FirstOrDefaultAsync();

            int gId = await _dbContext.AssigmentGroup
                .Where(n => n.GroupName == "Service Desk")
                .Select(i => i.GroupId)
                .FirstOrDefaultAsync();

            t.GroupId = gId;
            t.Assignee = _username;

            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

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
