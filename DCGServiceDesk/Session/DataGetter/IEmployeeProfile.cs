using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    public interface IEmployeeProfile
    {
        Task<List<string>> GetUserId(List<int> employeesId);
        Task<Employee> GetUser(string id);
        Task<int> GetEmployeeIdByUId(string id);
        Task<TimeZoneInfo> GetTimeZoneByUId(string uId);
    }
}
