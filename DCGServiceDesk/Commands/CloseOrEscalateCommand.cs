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
    public class CloseOrEscalateCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private NotEscalatedViewModel nEVM;
        private Brush Invalid = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private Brush Valid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        private List<bool> isFormValid = new List<bool>();
        private object request;

        public CloseOrEscalateCommand(DbInterfaceContainer dbInterfaces, NotEscalatedViewModel nEVM)
        {
            this.nEVM = nEVM;
            _requestQueue = dbInterfaces.RequestQueue;
            _userInfo = dbInterfaces.UserInfo;
            _employeeProfile = dbInterfaces.EmployeeProfile;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                _requestQueue.RefreshData();
                switch (parameter.ToString())
                {
                    case "Close":
                        await CloseRequest();
                        nEVM.RequestViewModel.RemoveAssignedRequest(request);
                        break;
                    case "Escalate":
                        await EscalateRequest();
                        nEVM.RequestViewModel.RemoveAssignedRequest(request);
                        break;
                    case "Refresh":
                        await RefreshRequest();
                        break;
                    case "Save":
                        await Save();
                        break;
                    case "SubmitMessage":
                        if(nEVM.RequestViewModel.Escalated.ConversationMessage.Length > 0)
                            await SubmitMessage();
                        else
                            nEVM.RequestViewModel.Escalated.ConvMsgValid = 
                                new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
        private async Task Save()
        {
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            string state = RequestService.GetStateName(request);

            switch (state)
            {
                case "Waiting":
                    await SaveWaiting();
                    break;
                case "Closed":
                    await CloseRequest();
                    break;
                case "Open":
                    await SaveOpen();
                    break;
            }
        }
        private async Task SaveWaiting()
        {

        }
        private async Task SaveOpen()
        {

        }
        private async Task SubmitMessage()
        {
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
            string message = nEVM.RequestViewModel.Escalated.ConversationMessage;
            int historyId = RequestService.GetHistoryId(request);
            await _requestQueue.AddNewMessage(historyId, message, 
                nEVM.RequestViewModel.GetUsername());
            nEVM.RequestViewModel.Escalated.FrocedMessageRemove();
            await RefreshRequest();
        }
        private async Task RefreshRequest()
        {
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
            Notification notification = new Notification();
            switch (requestType)
            {
                case "TaskRequestProxy":
                    TaskRequest t = (TaskRequest)request;
                    TaskRequest updatedT = await _requestQueue.GetTask(t.TaskId);
                    UpdateTModel(updatedT);
                    nEVM.RequestViewModel.Escalated.Notifications = 
                        notification.NotificationBuilder(updatedT.History.Status
                        .Where(n => n.NotNotification == false).ToList());
                    break;
                case "IncidentProxy":
                    Incident im = (Incident)request;
                    Incident updatedIM = await _requestQueue.GetIncident(im.IncidentId);
                    UpdateIMModel(updatedIM);
                    nEVM.RequestViewModel.Escalated.Notifications = 
                        notification.NotificationBuilder(updatedIM.History.Status
                        .Where(n => n.NotNotification == false).ToList());
                    break;
                case "ServiceRequestProxy":
                    ServiceRequest c = (ServiceRequest)request;
                    ServiceRequest updatedChange = await _requestQueue.GetChange(c.RequestId);
                    UpdateCModel(updatedChange);
                    nEVM.RequestViewModel.Escalated.Notifications = 
                        notification.NotificationBuilder(updatedChange.History.Status
                        .Where(n => n.NotNotification == false).ToList());
                    break;

            }
        }
        private void UpdateIMModel(Incident im)
        {
            nEVM.CurrentState = nEVM.States
                .Where(n => n.StateId == im.History.ActiveStatus.State.StateId)
                .FirstOrDefault();
            nEVM.CurrentImpact = nEVM.Impacts
                .Where(i => i.Id == im.Impact.Id)
                .FirstOrDefault();
            nEVM.CurrentSubcategory = nEVM.Categorizations
                .Where(s => s.ServiceId == im.Category.ServiceId)
                .FirstOrDefault();
            nEVM.CurrentUrgency = nEVM.Urgencies
                .Where(u => u.Id == im.Urgency.Id)
                .FirstOrDefault();
            nEVM.Topic = im.Topic;
            nEVM.Description = im.Description;
            nEVM.Solution = im.History.Solution;
            if(im.History.CloserDue != null)
            {
                nEVM.CloserDue = nEVM.CloserDues
                    .Where(d => d.CloserId == im.History.CloserDue.CloserId)
                    .FirstOrDefault();
            }
            else
            {
                nEVM.CloserDue = null; 
            }

        }
        private void UpdateCModel(ServiceRequest c)
        {

            nEVM.CurrentState = nEVM.States
                .Where(n => n.StateId == c.History.ActiveStatus.State.StateId)
                .FirstOrDefault();
            nEVM.CurrentImpact = nEVM.Impacts
                .Where(i => i.Id == c.Impact.Id)
                .FirstOrDefault();
            nEVM.CurrentSubcategory = nEVM.Categorizations
                .Where(s => s.ServiceId == c.Category.ServiceId)
                .FirstOrDefault();
            nEVM.CurrentUrgency = nEVM.Urgencies
                .Where(u => u.Id == c.Urgency.Id)
                .FirstOrDefault();
            nEVM.Topic = c.Topic;
            nEVM.Description = c.Description;
            nEVM.Solution = c.History.Solution;

            if (c.History.CloserDue != null)
            {
                nEVM.CloserDue = nEVM.CloserDues
                    .Where(d => d.CloserId == c.History.CloserDue.CloserId)
                    .FirstOrDefault();
            }
            else
            {
                nEVM.CloserDue = null;
            }

        }
        private void UpdateTModel(TaskRequest t)
        {
            nEVM.CurrentState = nEVM.States
                .Where(n => n.StateId == t.History.ActiveStatus.State.StateId)
                .FirstOrDefault();
            nEVM.CurrentImpact = nEVM.Impacts
                .Where(i => i.Id == t.Impact.Id)
                .FirstOrDefault();
            nEVM.CurrentSubcategory = nEVM.Categorizations
                .Where(s => s.ServiceId == t.Category.ServiceId)
                .FirstOrDefault();
            nEVM.CurrentUrgency = nEVM.Urgencies
                .Where(u => u.Id == t.Urgency.Id)
                .FirstOrDefault();
            nEVM.Topic = t.Topic;
            nEVM.Description = t.Description;
            nEVM.Solution = t.History.Solution;

            if (t.History.CloserDue != null)
            {
                nEVM.CloserDue = nEVM.CloserDues
                    .Where(d => d.CloserId == t.History.CloserDue.CloserId)
                    .FirstOrDefault();
            }
            else
            {
                nEVM.CloserDue = null;
            }
        }
        private async Task EscalateRequest()
        {
            string contactId = await _userInfo.GetUserId(nEVM.CUsername);
            string recipientId = await _userInfo.GetUserId(nEVM.RUsername);
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            CheckContactField(contactId);
            CheckRecipientField(recipientId);
            CheckTopicField();
            CheckDescField();
            if (!isFormValid.Contains(false))
            {
                string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
                switch (requestType)
                {
                    case "TaskRequestProxy":
                        TaskRequest task = await UpdateT(false);
                        nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests = task;
                        break;
                    case "IncidentProxy":
                        Incident incident = await UpdateIM(false);
                        nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests = incident;
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest change = await UpdateC(false);
                        nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests = change;
                        break;

                }
                nEVM.RequestViewModel.CurrentMode = nEVM.RequestViewModel.Escalation;
            }
            else
                isFormValid.Clear();
        }
        private async Task CloseRequest()
        {
            string contactId = await _userInfo.GetUserId(nEVM.CUsername);
            string recipientId = await _userInfo.GetUserId(nEVM.RUsername);
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            CheckContactField(contactId);
            CheckRecipientField(recipientId);
            CheckTopicField();
            CheckDescField();
            CheckSolutionField();
            CheckDueField();
            CheckStatusIfEscalated();
            if (!isFormValid.Contains(false))
            {
                string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
                switch (requestType)
                {
                    case "TaskRequestProxy":
                        TaskRequest task = await UpdateT(true);
                        await _requestQueue.UpdateTaskRequest(task, nEVM.AdminUsername, "Closed");
                        break;
                    case "IncidentProxy":
                        Incident incident = await UpdateIM(true);
                        await _requestQueue.UpdateIncident(incident, nEVM.AdminUsername, "Closed");
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest change = await UpdateC(true);
                        await _requestQueue.UpdateServiceRequest(change, nEVM.AdminUsername, "Closed");
                        break;

                }
                nEVM.RequestViewModel.RemoveCurrentTab();
            }
            else
                isFormValid.Clear();
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
        private async Task<Incident> UpdateIM(bool toClosed)
        {
            Incident im = RequestService.ConvertRequest(request);
            Status newStatus = im.History.ActiveStatus;
            newStatus.State = SetState(toClosed);
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
        private async Task<TaskRequest> UpdateT(bool toClosed)
        {
            TaskRequest t = RequestService.ConvertRequest(request);
            Status newStatus = t.History.ActiveStatus;
            newStatus.State = SetState(toClosed);
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
        private async Task<ServiceRequest> UpdateC(bool toClosed)
        {
            ServiceRequest c = RequestService.ConvertRequest(request);
            Status newStatus = c.History.ActiveStatus;
            newStatus.State = SetState(toClosed);
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
        private State SetState(bool isClosed)
        {
            if (isClosed)
               return nEVM.States.Where(n => n.StateName == "Closed").FirstOrDefault();
            else
               return nEVM.States.Where(n => n.StateName == "Open").FirstOrDefault();
        }
        private void CheckStatusIfEscalated()
        {
            if (nEVM.RequestViewModel.CurrentMode.GetType().Name == "EscalatedRequestViewModel")
                CheckStatusForClose();
        }
        private void CheckStatusForClose()
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
    }
}