using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.Data.Services
{
    public interface IEmployeeService
    {
        Task<List<string>> GetUserIdFromEmployee(List<int> employeesId);
        Task<Employee> GetUserProfile(string id);
        Task<int> GetIdByUId(string id);
        Task<TimeZoneInfo> GetUserTimeZone(string uId);
    }
}
