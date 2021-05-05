using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Services
{
    public interface IViewRequestService
    {
        Task<RequestInfo> SetAllRequestQueue(List<object> requests, string queueName, bool isGroup = false);
        Task<RequestInfo> SetIncidentQueue();
        Task<RequestInfo> SetChangesQueue();
        Task<RequestInfo> SetTasksQueue();
        Task<RequestInfo> SetAllIncidentsQueue(int id = 0, bool isSingle = false);
        Task<RequestInfo> SetAllChangesQueue(int id = 0, bool isSingle = false);
        Task<RequestInfo> SetAllTasksQueue(int id = 0, bool isSingle = false);
    }
}
