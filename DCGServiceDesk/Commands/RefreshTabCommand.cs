using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class RefreshTabCommand : AsyncCommandBase
    {
        private HomeViewModel _hVM;
        private Tab tab;
        private readonly IViewRequestService _viewRequestService;
        private readonly IRequestQueue _requestQueue;

        public RefreshTabCommand(Tab tab, IViewRequestService viewRequestService,
            IRequestQueue requestQueue)
        {
            this.tab = tab;
            _viewRequestService = viewRequestService;
            _requestQueue = requestQueue;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                _hVM = (HomeViewModel)parameter;
                _requestQueue.RefreshData();
                string choosen = tab.QueueType;

                switch (choosen)
                {
                    case "SD":
                        var requests = await _requestQueue.GetRequests();
                        _hVM.TabRefresh(tab, await _viewRequestService
                            .SetAllRequestQueue(requests, "Service Requests"));
                        break;
                    case "IM":
                        _hVM.TabRefresh(tab, await _viewRequestService.SetIncidentQueue());
                        break;
                    case "C":
                        _hVM.TabRefresh(tab, await _viewRequestService.SetChangesQueue());
                        break;
                    case "T":
                        _hVM.TabRefresh(tab, await _viewRequestService.SetTasksQueue());
                        break;
                    case "AR"://CurrentAssignedRequests
                        var assignedRequests = await _requestQueue.GetAssignedRequests(_hVM.loggedUser.ActiveUser);
                        _hVM.TabRefresh(tab, await _viewRequestService
                            .SetAllRequestQueue(assignedRequests, "Requests assigned"));
                        break;
                    case "G":
                        var groupRequests = await _requestQueue.GetGroupRequests(((AssigmentGroup)parameter).GroupId);
                        _hVM.TabRefresh(tab, await _viewRequestService
                            .SetAllRequestQueue(groupRequests, ((AssigmentGroup)parameter).GroupName, true));
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }

    }
}
