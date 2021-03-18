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
    }
}
