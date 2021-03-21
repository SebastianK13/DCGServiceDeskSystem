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
        private readonly string _username;
        private readonly IRequestQueue _requestQueue;
        private readonly HomeViewModel _hVM;
        public UpdateRequestCommand(HomeViewModel homeViewModel, IRequestQueue requestQueue, string username)
        {
            _hVM = homeViewModel;
            _requestQueue = requestQueue;
            _username = username;
        }
        public override async Task ExecuteAsync(object parameter)
        {
            var choosenRequest = ((RequestViewModel)parameter).WorkspaceInfo[0].ServiceRequests;
            string option = choosenRequest.GetType().Name;
            try
            {
                switch (option)
                {
                    case "IncidentProxy":
                        Incident im = (Incident)choosenRequest;
                        await UpdateRequestAssignee(im.IncidentId, "Incident", im);
                        RemoveFromQueue(im);
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest c = (ServiceRequest)choosenRequest;
                        await UpdateRequestAssignee(c.RequestId, "Change", c);
                        RemoveFromQueue(c);
                        break;
                    case "TaskRequestProxy":
                        TaskRequest t = (TaskRequest)choosenRequest;
                        await UpdateRequestAssignee(t.TaskId, "Task", t);
                        RemoveFromQueue(t);
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
        public async Task UpdateRequestAssignee(int rId, string requestType, object request) =>
            await _requestQueue.ChangeRequestAsignee(rId, requestType, _username);

        public void RemoveFromQueue(object request)
        {
            string requestType = request.GetType().Name;
            var choosenRequest = RequestService.ConvertRequest(request, requestType);
            int y = 0;
            foreach (var r in _hVM.Tabs)
            {
                if (r.GetType().Name == "QueueViewModel")
                {
                    foreach (var t in r.WorkspaceInfo)
                    {
                        string currentType = t.ServiceRequests.GetType().Name;

                        if (currentType == requestType)
                        {
                            var currentRequest = RequestService.ConvertRequest(t.ServiceRequests, requestType);

                            if (RequestService.GetId(choosenRequest) == RequestService.GetId(currentRequest))
                            {
                                r.WorkspaceInfo.Remove(t);
                                y++;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
