using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    public interface IRequestQueue
    {
        Task<List<object>> GetRequests();
        Task<List<Incident>> GetIncidents();
        Task<List<TaskRequest>> GetTasks();
        Task<List<ServiceRequest>> GetChanges();
        Task<bool> ChangeRequestAsignee(int requestId, string requestType, string username);
        Task<List<object>> GetGroupRequests(int groupId);
        List<AssigmentGroup> GetGroups(string activeUser);
    }
}
