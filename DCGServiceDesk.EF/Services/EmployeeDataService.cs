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
    public class EmployeeDataService : IEmployeeService
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private AppEmployeesDbContext _dbContext;

        public EmployeeDataService(IDatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
            _dbContext = _databaseContextFactory.CreateEmployeesDbContext();
        }

        public async Task<int> GetIdByUId(string id) =>
            await _dbContext.Employees
            .Where(u => u.UserId == id)
            .Select(i => i.EmployeeId)
            .FirstOrDefaultAsync();

        public async Task<List<string>> GetUserIdFromEmployee(List<int> eIds)
        {
            List<string> usernamesList = new List<string>();

            foreach (var e in eIds)
            {
                usernamesList.Add(await _dbContext.Employees
                    .Where(x => x.EmployeeId == e)
                    .Select(x => x.UserId)
                    .FirstOrDefaultAsync());
            }

            return usernamesList;
        }

        public async Task<Employee> GetUserProfile(string id) =>
            await _dbContext.Employees
            .Where(i => i.UserId == id)
            .FirstOrDefaultAsync();

        public async Task<TimeZoneInfo> GetUserTimeZone(string uId)
        {
            var user = await _dbContext.Employees.Where(e => e.UserId == uId).FirstOrDefaultAsync();
            return TimeZoneInfo.FindSystemTimeZoneById(user.Zone.TimeZoneID);
        }

        public async Task<string> GetUIdById(int id) =>
            await _dbContext.Employees
            .Where(i => i.EmployeeId == id)
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();
    }




}

