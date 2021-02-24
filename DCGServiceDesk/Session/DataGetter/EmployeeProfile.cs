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

        public EmployeeProfile(IEmployeeService employeeProfile)
        {
            _employeeService = employeeProfile;
        }
        public async Task<List<string>> GetUserId(List<int> employeesId) =>
            await _employeeService.GetUserIdFromEmployee(employeesId);
    }
}
