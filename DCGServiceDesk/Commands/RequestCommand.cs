using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.Session.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class RequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IQueueCreator _queueCreator;

        public RequestCommand(IRequestQueue requestQueue, IQueueCreator queueCreator)
        {
            _requestQueue = requestQueue;
            _queueCreator = queueCreator;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                string choosen = parameter as string;
                switch (choosen)
                {
                    case "RequestQueue":
                        var requests = await _requestQueue.GetRequests();
                        _queueCreator.GenerateRequestQueue(requests);
                        break;
                    case "IncidentManagment":
                        break;
                    case "ChangeManagment":
                        break;
                    case "TaskManagment":
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {

            }
        }
    }
}
