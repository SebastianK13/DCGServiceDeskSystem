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

        public async Task<List<ServiceRequest>> GetAll()
        {

            return await _dbContext.Applications.ToListAsync();
        }

        public async Task<List<Incident>> GetAllIncidents() => 
            await _dbContext.Incidents.ToListAsync();

        public async Task<List<TaskRequest>> GetAllTasks() => 
            await _dbContext.Tasks.ToListAsync();


        //public async Task<User> GetByName(string username)
        //{

        //    var user = await _dbContext.AspNetUsers
        //     .Select(u => new User { Username = u.UserName, Password = u.PasswordHash })
        //     .FirstOrDefaultAsync(u => u.Username == username);

        //    return user;
        //}



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
