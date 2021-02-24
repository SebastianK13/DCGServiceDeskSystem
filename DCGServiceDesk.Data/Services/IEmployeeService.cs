using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Services
{
    public interface IEmployeeService
    {
        Task<List<string>> GetUserIdFromEmployee(List<int> employeesId);
    }
}
