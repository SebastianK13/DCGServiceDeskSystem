using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.Session.Queue
{
    public class QueueCreator : IQueueCreator
    {
        public void GenerateRequestQueue(List<ServiceRequest> requests)
        {

        }

        public void GenerateChangeQueue()
        {
            throw new NotImplementedException();
        }

        public void GenerateIncidentQueue()
        {
            throw new NotImplementedException();
        }
        public void GenerateTaskQueue()
        {
            throw new NotImplementedException();
        }
    }
}
