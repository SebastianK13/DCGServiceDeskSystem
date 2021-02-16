using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.Controls.Tab.Model
{
    public class TabContent:Tab
    {
        public TabContent(List<ServiceRequest> changes, string label)
        {
            ServiceRequests = new List<object>(changes);
            Label = label;
        }
        public TabContent(List<Incident> incidents, string label)
        {
            ServiceRequests = new List<object>(incidents);
            Label = label;
        }       
        public TabContent(List<TaskRequest> tasks, string label)
        {
            ServiceRequests = new List<object>(tasks);
            Label = label;
        }        
        public TabContent(List<object> requests, string label)
        {
            ServiceRequests = new List<object>(requests);
            Label = label;
        }
    }
}
