using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class RequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private HomeViewModel _hVM;

        public RequestCommand(IRequestQueue requestQueue, HomeViewModel hVM)
        {
            _requestQueue = requestQueue;
            _hVM = hVM;
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
                        _hVM.SetRequests(requests);
                        break;
                    case "IncidentQueue":
                        var incidents = await _requestQueue.GetIncidents();
                        _hVM.SetIncidents(incidents);
                        break;
                    case "ChangesQueue":
                        var changes = await _requestQueue.GetChanges();
                        _hVM.SetChanges(changes);
                        break;
                    case "TasksQueue":
                        var tasks = await _requestQueue.GetTasks();
                        _hVM.SetTasks(tasks);
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
