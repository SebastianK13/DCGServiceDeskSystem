﻿using DCGServiceDesk.Data.Models;
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
        public Task<User> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByName(string username)
        {
            
            var user = await _dbContext.AspNetUsers
             .Select(u => new User{ Username = u.UserName, Password = u.PasswordHash })
             .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }



        public Task<bool> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(int id, User entity)
        {
            throw new NotImplementedException();
        }
    }
}