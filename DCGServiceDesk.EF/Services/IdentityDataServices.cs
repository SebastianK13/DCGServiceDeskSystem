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
    public class IdentityDataServices : IUserService
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private AppIdentityDbContext _dbContext;

        public IdentityDataServices(IDatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
            _dbContext = _databaseContextFactory.CreateIdentityDbContext();
        }

        public async Task<User> GetByName(string username)
        {
            
            var user = await _dbContext.AspNetUsers
             .Select(u => new User{ Username = u.UserName, Password = u.PasswordHash })
             .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        public async Task<List<CommunicationInfo>> GetUserName(List<string> eIdContact, List<string> eIdRequested)
        {
            List<CommunicationInfo> communicationInfo = new List<CommunicationInfo>();

            for (int i = 0; i < eIdRequested.Count(); i++)
            {
                communicationInfo.Add(new CommunicationInfo
                {
                    RequestedPerson = await _dbContext.AspNetUsers
                    .Where(u => u.Id == eIdRequested[i])
                    .Select(u => u.UserName).FirstOrDefaultAsync(),

                    ContactPerson = await _dbContext.AspNetUsers
                    .Where(u => u.Id == eIdContact[i])
                    .Select(u => u.UserName).FirstOrDefaultAsync()
                });                    
            }

            return communicationInfo;
        }

        public async Task<string> GetUserId(string username) =>
            await _dbContext.AspNetUsers
            .Where(u => u.UserName == username)
            .Select(i => i.Id)
            .FirstOrDefaultAsync();

        public async Task<string> GetUserNameById(string userId) =>
            await _dbContext.AspNetUsers
            .Where(i => i.Id == userId)
            .Select(u=>u.UserName)
            .FirstOrDefaultAsync();

        public Task<bool> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(int id, User entity)
        {
            throw new NotImplementedException();
        }
        public Task<User> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}