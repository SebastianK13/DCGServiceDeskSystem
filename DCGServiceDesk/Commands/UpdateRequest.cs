using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
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
    public class UpdateRequest : AsyncCommandBase
    {
        private readonly string _username;
        private readonly IRequestQueue _requestQueue;
        private readonly HomeViewModel _hVM;
        public UpdateRequest(HomeViewModel homeViewModel, IRequestQueue requestQueue, string username)
        {
            _hVM = homeViewModel;
            _requestQueue = requestQueue;
            _username = username;
        }
        public override async Task ExecuteAsync(object parameter)
        {
            string option = parameter.GetType().Name;
            try
            {
                switch (option)
                {
                    case "IncidentProxy":
                        await ChangeCurrentIMVisibility(((Incident)parameter).IncidentId);
                        break;
                    case "ServiceRequestProxy":
                        await ChangeCurrentCVisibility(((ServiceRequest)parameter).RequestId);
                        break;
                    case "TaskRequestProxy":
                        await ChangeCurrentTVisibility(((TaskRequest)parameter).TaskId);
                        break;
                }

            }
            catch (Exception e)
            {

            }
        }
        public async Task<bool> ChangeCurrentIMVisibility(int iId)
        {
            bool result = false;
            foreach (var r in _hVM.Tabs)
            {
                if (r.GetType().Name == "QueueViewModel")
                {
                    foreach (var t in r.WorkspaceInfo)
                    {
                        string currentType = t.ServiceRequests.GetType().Name;

                        if (currentType == "IncidentProxy" && ((Incident)t.ServiceRequests).IncidentId == iId)
                        {
                            t.RequestVisibility = false;
                            result = await _requestQueue.ChangeRequestAsignee(iId, "Incident", _username);
                        }
                    }
                }
            }
            return result;
        }
        public async Task<bool> ChangeCurrentCVisibility(int cId)
        {
            bool result = false;
            foreach (var r in _hVM.Tabs)
            {
                if(r.GetType().Name == "QueueViewModel")
                {
                    foreach (var t in r.WorkspaceInfo)
                    {
                        string currentType = t.ServiceRequests.GetType().Name;

                        if (currentType == "ServiceRequestProxy" && ((ServiceRequest)t.ServiceRequests).RequestId == cId)
                        {
                            t.RequestVisibility = false;
                            result = await _requestQueue.ChangeRequestAsignee(cId, "Change", _username);
                        }
                    }
                }

            }
            return result;
        }
        public async Task<bool> ChangeCurrentTVisibility(int tId)
        {
            bool result = false;
            foreach (var r in _hVM.Tabs)
            {
                if (r.GetType().Name == "QueueViewModel")
                {
                    foreach (var t in r.WorkspaceInfo)
                    {
                        string currentType = t.ServiceRequests.GetType().Name;

                            if (currentType == "TaskRequestProxy" && ((TaskRequest)t.ServiceRequests).TaskId == tId)
                        {
                            t.RequestVisibility = false;
                            result = await _requestQueue.ChangeRequestAsignee(tId, "Task", _username);
                        }
                    }
                }
            }
            return result;
        }

    }
}
