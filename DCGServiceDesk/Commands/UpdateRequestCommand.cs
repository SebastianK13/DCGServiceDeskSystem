using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class UpdateRequestCommand : AsyncCommandBase
    {
        private string _username;
        private readonly IRequestQueue _requestQueue;
        private readonly NotEscalatedViewModel _nEVM;
        public UpdateRequestCommand(IRequestQueue requestQueue, NotEscalatedViewModel nEVM)
        {
            _requestQueue = requestQueue;
            _nEVM = nEVM;
        }
        public override async Task ExecuteAsync(object parameter)
        {
            try
            {
                string option = parameter.GetType().Name;
                _username = _nEVM.RequestViewModel.GetUsername();

                switch (option)
                {
                    case "IncidentProxy":
                        Incident im = (Incident)parameter;
                        await UpdateRequestAssignee(im.IncidentId, "Incident", im);
                        _nEVM.RequestViewModel.RemoveAssignedRequest(im);
                        await _nEVM.CheckIfRequestAssigned();
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest c = (ServiceRequest)parameter;
                        await UpdateRequestAssignee(c.RequestId, "Change", c);
                        _nEVM.RequestViewModel.RemoveAssignedRequest(c);
                        await _nEVM.CheckIfRequestAssigned();
                        break;
                    case "TaskRequestProxy":
                        TaskRequest t = (TaskRequest)parameter;
                        await UpdateRequestAssignee(t.TaskId, "Task", t);
                        _nEVM.RequestViewModel.RemoveAssignedRequest(t);
                        await _nEVM.CheckIfRequestAssigned();
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
        public async Task UpdateRequestAssignee(int rId, string requestType, object request)
        { 
            if(_username != null && _username != "")
            {
                await _requestQueue.ChangeRequestAsignee(rId, requestType, _username);
            }

        }
            

    }
}
