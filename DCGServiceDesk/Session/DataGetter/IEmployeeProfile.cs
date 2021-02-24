using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    public interface IEmployeeProfile
    {
        Task<List<string>> GetUserId(List<int> employeesId);
    }
}
