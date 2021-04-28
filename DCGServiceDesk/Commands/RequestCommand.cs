using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
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
    public class RequestCommand : AsyncCommandBase
    {
        private readonly DbInterfaceContainer _interfaceContainer;
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private readonly IViewRequestService _viewRequestService;
        private HomeViewModel _hVM;
        private List<string> ClassTypes = new List<string> { "IncidentProxy", "ServiceRequestProxy", "TaskRequestProxy" };

        public RequestCommand(IRequestQueue requestQueue, IUserInfo userInfo, IEmployeeProfile employeeProfile, HomeViewModel hVM)
        {
            _interfaceContainer = new DbInterfaceContainer(requestQueue, userInfo, employeeProfile);
            _requestQueue = requestQueue;
            _userInfo = userInfo;
            _employeeProfile = employeeProfile;
            _hVM = hVM;
            _viewRequestService = new ViewRequestService(_interfaceContainer);
        }
        public async override Task ExecuteAsync(object parameter)
        {
            try
            {                
                _requestQueue.RefreshData();
                string choosen = parameter as string;
                if (parameter.GetType().Name == "TabContainer")
                {
                    choosen = ((TabContainer)parameter).ServiceRequests.GetType().Name;
                }
                else if(parameter.GetType().Name == "AssigmentGroupProxy")
                {
                    choosen = "Group";
                }

                switch (choosen)
                {
                    case "RequestQueue":
                        var requests = await _requestQueue.GetRequests();
                        _hVM.SetRequestsQueue(
                            await _viewRequestService.SetAllRequestQueue(requests, "Service Requests"));
                        break;
                    case "IncidentQueue":
                        _hVM.SetRequestsQueue(
                            await _viewRequestService.SetIncidentQueue());
                        break;
                    case "ChangesQueue":
                        _hVM.SetRequestsQueue(
                            await _viewRequestService.SetChangesQueue());
                        break;
                    case "TasksQueue":
                        _hVM.SetRequestsQueue(
                            await _viewRequestService.SetTasksQueue());
                        break;
                    case "CurrentAssignRequests":
                        var assignedRequests = await _requestQueue.GetAssignedRequests(_hVM.loggedUser.ActiveUser);
                        _hVM.SetRequestsQueue(
                            await _viewRequestService.SetAllRequestQueue(assignedRequests, "Requests assigned"));
                        break;
                    case "Group":
                        var groupRequests = await _requestQueue.GetGroupRequests(((AssigmentGroup)parameter).GroupId);
                        _hVM.SetRequestsQueue(
                            await _viewRequestService.SetAllRequestQueue(groupRequests, ((AssigmentGroup)parameter).GroupName, true));
                        break;
                    case "IncidentProxy":
                        await EscalatedOrNot(parameter, "IM");
                        break;
                    case "ServiceRequestProxy":
                        await EscalatedOrNot(parameter, "C");
                        break;
                    case "TaskRequestProxy":
                        await EscalatedOrNot(parameter, "T");
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {

            }
        }
        private async Task EscalatedOrNot(object parameter, string requestType)
        {
            var groups = await _requestQueue.GetGroups();
            SingleRequestInfo request = new SingleRequestInfo {Groups = groups};
            bool isEscalated = false;
            Notification notification = new Notification();

            switch (requestType)
            {
                case "T":
                    var t = RequestService.ExtractTask(parameter);
                    TaskRequest updatedTask = await _requestQueue.GetTask(t.TaskId);
                    string labelT = RequestService.SetRequestId(t.TaskId, "T");
                    request.Request = updatedTask;
                    request.Info = RequestService.ExtractAdditionalInfo(parameter);
                    request.Label = labelT;

                    if (t.Group == null || t.Group.GroupName == "Service Desk")
                    {
                        RequestViewModel rVM = new RequestViewModel(request, _interfaceContainer, _hVM, isEscalated);
                        await rVM.InitializeNEVMModel();
                        await rVM.InitializeEVMModel();
                        _hVM.Tabs.Add(rVM);
                    }
                    else
                    {
                        isEscalated = true;
                        RequestViewModel rVM = new RequestViewModel(request, _interfaceContainer, _hVM, isEscalated);
                        await rVM.InitializeNEVMModel();
                        await rVM.InitializeEVMModel();
                        rVM.InitializeEscalatedModel();
                        rVM.Escalated.Notifications = notification
                            .NotificationBuilder(rVM.Escalated.Statuses.Where(n=>n.NotNotification == false)
                            .OrderBy(d => d.CreateDate)
                            .ToList());
                        _hVM.Tabs.Add(rVM);
                    }

                    break;
                case "IM":
                    var im = RequestService.ExtractIncident(parameter);
                    Incident updatedIncident = await _requestQueue.GetIncident(im.IncidentId);
                    string labelIM = RequestService.SetRequestId(im.IncidentId, "IM");
                    request.Request = updatedIncident;
                    request.Info = RequestService.ExtractAdditionalInfo(parameter);
                    request.Label = labelIM;
                    if (im.Group == null || im.Group.GroupName == "Service Desk")
                    {
                        RequestViewModel rVM = new RequestViewModel(request, _interfaceContainer, _hVM, isEscalated);
                        await rVM.InitializeNEVMModel();
                        await rVM.InitializeEVMModel();
                        _hVM.Tabs.Add(rVM);
                    }
                    else
                    {
                        isEscalated = true;
                        RequestViewModel rVM = new RequestViewModel(request, _interfaceContainer, _hVM, isEscalated);
                        await rVM.InitializeNEVMModel();
                        await rVM.InitializeEVMModel();
                        rVM.InitializeEscalatedModel();
                        rVM.Escalated.Notifications = notification
                            .NotificationBuilder(rVM.Escalated.Statuses.Where(n => n.NotNotification == false)
                            .OrderBy(d => d.CreateDate)
                            .ToList());
                        _hVM.Tabs.Add(rVM);
                    }
                    break;
                case "C":
                    var c = RequestService.ExtractChange(parameter);
                    ServiceRequest updatedChange = await _requestQueue.GetChange(c.RequestId);
                    string labelC = RequestService.SetRequestId(c.RequestId, "C");
                    request.Request = updatedChange;
                    request.Info = RequestService.ExtractAdditionalInfo(parameter);
                    request.Label = labelC;
                    if (c.Group == null || c.Group.GroupName == "Service Desk")
                    {
                        RequestViewModel rVM = new RequestViewModel(request, _interfaceContainer, _hVM, isEscalated);
                        await rVM.InitializeNEVMModel();
                        await rVM.InitializeEVMModel();
                        _hVM.Tabs.Add(rVM);
                    }
                    else
                    {
                        isEscalated = true;
                        RequestViewModel rVM = new RequestViewModel(request, _interfaceContainer, _hVM, isEscalated);
                        await rVM.InitializeNEVMModel();
                        await rVM.InitializeEVMModel();
                        rVM.InitializeEscalatedModel();
                        rVM.Escalated.Notifications = notification
                            .NotificationBuilder(rVM.Escalated.Statuses.Where(n => n.NotNotification == false)
                            .OrderBy(d => d.CreateDate)
                            .ToList());
                        _hVM.Tabs.Add(rVM);
                    }
                    break;
            }
        }
    }
}
