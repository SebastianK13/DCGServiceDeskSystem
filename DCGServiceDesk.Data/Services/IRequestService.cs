using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Services
{
    public interface IRequestService: ICrud<ServiceRequest>
    {
        Task<List<ServiceRequest>> GetAll();
    }
}
