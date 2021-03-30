using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;

namespace DCGServiceDesk.Commands
{
    public class CloseRequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private NotEscalatedViewModel nEVM;
        private Brush Invalid = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private Brush Valid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        private List<bool> isFormValid = new List<bool>();
        private object request;

        public CloseRequestCommand(DbInterfaceContainer dbInterfaces, NotEscalatedViewModel nEVM)
        {
            this.nEVM = nEVM;
            _requestQueue = dbInterfaces.RequestQueue;
            _userInfo = dbInterfaces.UserInfo;
            _employeeProfile = dbInterfaces.EmployeeProfile;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            string contactId = await _userInfo.GetUserId(nEVM.CUsername);
            string recipientId = await _userInfo.GetUserId(nEVM.RUsername);
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            CheckStatus();
            CheckContactField(contactId);
            CheckRecipientField(recipientId);
            CheckTopicField();
            CheckDescField();
            CheckSolutionField();
            CheckDueField();
            if (!isFormValid.Contains(false))
            {
                string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
                switch (requestType)
                {
                    case "TaskRequestProxy":
                        TaskRequest task = await UpdateT();
                        await _requestQueue.UpdateTaskRequest(task);
                        break;
                    case "IncidentProxy":
                        Incident incident = await UpdateIM();
                        await _requestQueue.UpdateIncident(incident); 
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest change = await UpdateC();
                        await _requestQueue.UpdateServiceRequest(change);
                        break;

                }
            }
            else
                isFormValid.Clear();
        }
        private void CheckStatus()
        {
            if (nEVM.CurrentState != nEVM.States.Where(s => s.StateName == "Closed").FirstOrDefault())
            {
                nEVM.StatusValid = Invalid;
                isFormValid.Add(false);
            }
            else
            {
                isFormValid.Add(true);
                nEVM.StatusValid = Valid;
            }

        }
        private void CheckContactField(string contactId)
        {           
            if (contactId != "" && contactId != null)
            {
                nEVM.ContactValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.ContactValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckRecipientField(string recipientId)
        {
            if (recipientId != "" && recipientId != null)
            {
                nEVM.RecipientValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.RecipientValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckTopicField()
        {
            if (nEVM.Topic != "" && nEVM.Topic != null)
            {
                isFormValid.Add(true);
                nEVM.TitleValid = Valid;
            }
            else
            {
                nEVM.TitleValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckDescField()
        {
            if (nEVM.Description != "" && nEVM.Description != null)
            {
                nEVM.DescriptionValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.DescriptionValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckSolutionField()
        {
            if (nEVM.Solution != "" && nEVM.Solution != null)
            {
                nEVM.SolutionValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.SolutionValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckDueField()
        {
            if (nEVM.CloserDue != null)
            {
                nEVM.CloserDueValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.CloserDueValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private async Task<Incident> UpdateIM()
        {
            Incident im = RequestService.ConvertRequest(request);
            Status newStatus = im.History.ActiveStatus;
            newStatus.State = nEVM.CurrentState;
            im.History.Status.Add(newStatus);
            im.History.ActiveStatus = newStatus;
            im.History.CloserDue = nEVM.CloserDue;
            im.History.Solution = nEVM.Solution;
            im.RequestedPerson = await GetEmployeeId(nEVM.CUsername);
            im.ContactPerson = await GetEmployeeId(nEVM.RUsername);
            im.Category = nEVM.CurrentSubcategory;
            im.Impact = nEVM.CurrentImpact;
            im.Urgency = nEVM.CurrentUrgency;
            im.Priority = nEVM.CurrentPriority;
            im.Topic = nEVM.Topic;
            im.Description = nEVM.Description;

            return im;
        }
        private async Task<TaskRequest> UpdateT()
        {
            TaskRequest t = RequestService.ConvertRequest(request);
            Status newStatus = t.History.ActiveStatus;
            newStatus.State = nEVM.CurrentState;
            t.History.Status.Add(newStatus);
            t.History.ActiveStatus = newStatus;
            t.History.CloserDue = nEVM.CloserDue;
            t.History.Solution = nEVM.Solution;
            t.RequestedPerson = await GetEmployeeId(nEVM.CUsername);
            t.ContactPerson = await GetEmployeeId(nEVM.RUsername);
            t.Category = nEVM.CurrentSubcategory;
            t.Impact = nEVM.CurrentImpact;
            t.Urgency = nEVM.CurrentUrgency;
            t.Priority = nEVM.CurrentPriority;
            t.Topic = nEVM.Topic;
            t.Description = nEVM.Description;

            return t;
        }
        private async Task<ServiceRequest> UpdateC()
        {
            ServiceRequest c = RequestService.ConvertRequest(request);
            Status newStatus = c.History.ActiveStatus;
            newStatus.State = nEVM.CurrentState;
            c.History.Status.Add(newStatus);
            c.History.ActiveStatus = newStatus;
            c.History.CloserDue = nEVM.CloserDue;
            c.History.Solution = nEVM.Solution;
            c.RequestedPerson = await GetEmployeeId(nEVM.CUsername);
            c.ContactPerson = await GetEmployeeId(nEVM.RUsername);
            c.Category = nEVM.CurrentSubcategory;
            c.Impact = nEVM.CurrentImpact;
            c.Urgency = nEVM.CurrentUrgency;
            c.Priority = nEVM.CurrentPriority;
            c.Topic = nEVM.Topic;
            c.Description = nEVM.Description;

            return c;
        }
        private async Task<int> GetEmployeeId(string userName)
        {
            string userId = await _userInfo.GetUserId(userName);
            return await _employeeProfile.GetEmployeeIdByUId(userId);
        }
    }
}
