using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class ForwardOrDropCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly EscalationViewModel _eVM;
        private string _username;
        private object request;
        public ForwardOrDropCommand(IRequestQueue requestQueue, EscalationViewModel eVM)
        {
            _eVM = eVM;
            _requestQueue = requestQueue;
        }
        //Finish escalation or cancel and back to NotEscalatedView
        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                _requestQueue.RefreshData();
                _username = _eVM.RequestViewModel.GetUsername();
                switch (parameter.ToString())
                {
                    case "Forward":
                        await CheckAndForward();
                        _eVM.RequestViewModel.RemoveCurrentTab();
                        _eVM.RequestViewModel.RemoveAssignedRequest(request);
                        break;
                    case "Abandon":
                        _eVM.RequestViewModel.CurrentMode = _eVM.RequestViewModel.NotEscalated;
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
        private async Task CheckAndForward()
        {
            request = _eVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            AdditionalUpdateInfo additional = new AdditionalUpdateInfo();
            additional.Username = _username;
            switch (_eVM.designation)
            {
                case "TaskRequestProxy":
                    TaskRequest t = RequestService.SetAssigmentGroupT((TaskRequest)request, _eVM.ChoosenGroup);
                    t.History.ActiveStatus.Notification = t.History.ActiveStatus.Notification + t.Group.GroupName;
                    await _requestQueue.UpdateTaskRequest(t, additional);
                    break;
                case "IncidentProxy":
                    if (_eVM.IsAssociated)
                    {
                        await _requestQueue.AddAssociatedIM((Incident)request, _username, _eVM.ChoosenIncident);
                    }
                    else
                    {
                        Incident im = RequestService.SetAssigmentGroupIM((Incident)request, _eVM.ChoosenGroup);
                        im.History.ActiveStatus.Notification = im.History.ActiveStatus.Notification + im.Group.GroupName;
                        await _requestQueue.UpdateIncident((Incident)request, additional);
                    }
                    break;
                case "ServiceRequestProxy":
                    ServiceRequest c = RequestService.SetAssigmentGroupC((ServiceRequest)request, _eVM.ChoosenGroup);
                    c.History.ActiveStatus.Notification = c.History.ActiveStatus.Notification + c.Group.GroupName;
                    await _requestQueue.UpdateServiceRequest(c, additional);
                    break;

            }
        }
    }
}
