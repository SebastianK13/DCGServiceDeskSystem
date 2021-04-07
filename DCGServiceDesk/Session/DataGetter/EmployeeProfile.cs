using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    public class EmployeeProfile : IEmployeeProfile
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeProfile(IEmployeeService employeeProfile) =>
            _employeeService = employeeProfile;

        public async Task<int> GetEmployeeIdByUId(string id) =>
            await _employeeService.GetIdByUId(id);

        public async Task<Employee> GetUser(string id) =>
            await _employeeService.GetUserProfile(id);

        public async Task<List<string>> GetUserId(List<int> employeesId) =>
            await _employeeService.GetUserIdFromEmployee(employeesId);

        public async Task<TimeZoneInfo> GetTimeZoneByUId(string uId) =>
            await _employeeService.GetUserTimeZone(uId);
    }
}
