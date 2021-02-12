using System;
using System.Collections.Generic;
using System.Text;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.Session.Queue
{
    public interface IQueueCreator
    {
        void GenerateRequestQueue(List<ServiceRequest> requests);
        void GenerateIncidentQueue();
        void GenerateChangeQueue();
        void GenerateTaskQueue();
    }
}
