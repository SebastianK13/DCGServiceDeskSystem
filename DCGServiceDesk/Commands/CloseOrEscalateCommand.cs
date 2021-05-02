using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
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
        private Employee _user;
        private string superiorUsername;
        private bool isEscalated = false;
        private int hasGroupChanged = 0;
        private List<string> usernames = new List<string>();

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
                if (nEVM.RequestViewModel.Escalated != null)
                    await CheckUsersField();
                AdUsernames();
                switch (parameter.ToString())
                {
                    case "Close":
                        if (nEVM.RequestViewModel.Escalated != null)
                            isEscalated = true;
                        else
                            isEscalated = false;
                        await CloseRequest();
                        break;
                    case "Escalate":
                        isEscalated = false;
                        await EscalateRequest();
                        break;
                    case "Refresh":
                        await RefreshRequest();
                        break;
                    case "Save":
                        isEscalated = true;
                        await Save();
                        break;
                    case "SubmitMessage":
                        if (nEVM.RequestViewModel.Escalated.ConversationMessage.Length > 0)
                            await SubmitMessage();
                        else
                        {
                            nEVM.RequestViewModel.Escalated.ConvMsgValid =
                                new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            isFormValid.Add(false);
                        }
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
        private void AdUsernames()
        {
            usernames.Clear();
            usernames.Add(nEVM.RUsername);
            usernames.Add(nEVM.CUsername);
        }
        private async Task CheckUsersField()
        {
            await FindUser("FindAssignee");
            await FindUser("FindContact");
            await FindUser("FindRecipient");
        }
        private async Task FindUser(string parameter)
        {
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            string id = "";
            switch (parameter)
            {
                case "FindRecipient":
                    id = await _userInfo.GetUserId(nEVM.RUsername);
                    nEVM.RecipientValid = await GenerateProperColorAsync(id);
                    if (_user == null)
                        isFormValid.Add(false);
                    break;
                case "FindContact":
                    id = await _userInfo.GetUserId(nEVM.CUsername);
                    nEVM.ContactValid = await GenerateProperColorAsync(id);
                    if (_user == null)
                        isFormValid.Add(false);
                    break;
                case "FindAssignee":
                    id = await _userInfo.GetUserId(nEVM.RequestViewModel.Escalated.AUsername);
                    if (_user == null && (id != null && id != ""))
                        isFormValid.Add(false);
                    break;
            }
        }
        private async Task<SolidColorBrush> GenerateProperColorAsync(string id)
        {
            if (id != "" && id != null)
            {
                _user = await _employeeProfile.GetUser(id);
                superiorUsername = await _userInfo.GetUserNameById(_user.Superior?.UserId);
                return new SolidColorBrush(Color.FromRgb(171, 173, 179));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }
        private async Task Save()
        {
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            string state = nEVM.CurrentState.StateName;

            switch (state)
            {
                case "Waiting":
                    await SaveChanges("Waiting");
                    break;
                case "Closed":
                    await CloseRequest();
                    break;
                case "Open":
                    await SaveChanges("Open");
                    break;
            }
        }
        private async Task SaveChanges(string stateName)
        {
            string contactId = await _userInfo.GetUserId(nEVM.CUsername);
            string recipientId = await _userInfo.GetUserId(nEVM.RUsername);
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            AdditionalUpdateInfo additional = new AdditionalUpdateInfo();
            additional.Username = nEVM.AdminUsername;
            additional.Phase = stateName;
            CheckContactField(contactId);
            CheckRecipientField(recipientId);
            await CheckGroupMember();
            if (stateName == "Waiting")
                await CheckTimeFields();

            if (!isFormValid.Contains(false))
            {
                string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
                switch (requestType)
                {
                    case "TaskRequestProxy":
                        TaskRequest task = await UpdateT(stateName);
                        additional.Notification = task.History.ActiveStatus.Notification;
                        await _requestQueue.UpdateTaskRequest(task, additional);
                        break;
                    case "IncidentProxy":
                        Incident incident = await UpdateIM(stateName);
                        additional.Notification = incident.History.ActiveStatus.Notification;
                        await _requestQueue.UpdateIncident(incident, additional);
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest change = await UpdateC(stateName);
                        additional.Notification = change.History.ActiveStatus.Notification;
                        await _requestQueue.UpdateServiceRequest(change, additional);
                        break;

                }
                if (hasGroupChanged != 0)
                    nEVM.RequestViewModel.RemoveAssignedRequest(request);

                await RefreshRequest();
            }
            else
                isFormValid.Clear();
        }
        private async Task CheckTimeFields()
        {
            if (nEVM.isTimeValid)
            {
                if (!nEVM.RequestViewModel.Escalated.IsMsgTyped &&
                 (nEVM.RequestViewModel.Escalated.ConversationMessage != null ||
                 nEVM.RequestViewModel.Escalated.ConversationMessage?.Length > 0))
                {
                    await SubmitMessage();
                    isFormValid.Add(true);
                    nEVM.RequestViewModel.Escalated.ConvMsgValid = Valid;
                }
                else if (nEVM.RequestViewModel.Escalated.IsMsgTyped)
                {
                    isFormValid.Add(true);
                    nEVM.RequestViewModel.Escalated.ConvMsgValid = Valid;
                }
                else
                {
                    isFormValid.Add(false);
                    nEVM.RequestViewModel.Escalated.SelectedTab = 2;
                    nEVM.RequestViewModel.Escalated.ConvMsgValid = Invalid;
                }
                isFormValid.Add(true);
            }
            else
            {
                isFormValid.Add(false);
                nEVM.WaitingTimeValid = Invalid;
            }

        }
        private async Task CheckGroupMember()
        {
            if (nEVM.RequestViewModel.Escalated.AUsername != "" &&
                nEVM.RequestViewModel.Escalated.AUsername != null)
            {
                string id = await _userInfo.GetUserId(nEVM.RequestViewModel.Escalated.AUsername);
                nEVM.RequestViewModel.Escalated.AssigneeValid =
                    await GenerateAssigneeColor(id,
                    await CheckIsMember(nEVM.RequestViewModel.Escalated.AUsername));
                if (_user != null)
                {
                    nEVM.RequestViewModel.Escalated.Assignee = new AccountInfo(_user, superiorUsername);
                    nEVM.RequestViewModel.Escalated.FindAssigneeEventArea = true;
                }
                else
                {
                    nEVM.RequestViewModel.Escalated.Assignee = null;
                    isFormValid.Add(false);
                }
            }

        }
        private async Task<SolidColorBrush> GenerateAssigneeColor(string id, bool exist)
        {
            if (exist)
            {
                _user = await _employeeProfile.GetUser(id);
                superiorUsername = await _userInfo.GetUserNameById(_user.Superior.UserId);
                return new SolidColorBrush(Color.FromRgb(171, 173, 179));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }
        private async Task<bool> CheckIsMember(string username) =>
            await _requestQueue.IsGroupMember(username, nEVM.RequestViewModel.Escalated.ChoosenGroup.GroupId);
        private async Task SubmitMessage()
        {
            string message = nEVM.RequestViewModel.Escalated.ConversationMessage;

            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
            int historyId = RequestService.GetHistoryId(request);
            await _requestQueue.AddNewMessage(historyId, message,
                nEVM.RequestViewModel.GetUsername());
            nEVM.RequestViewModel.Escalated.ConversationMessage = "";
            nEVM.RequestViewModel.Escalated.IsMsgTyped = true;
        }
        private async Task RefreshRequest()
        {
            request = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests;
            string requestType = nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().RequestType;
            switch (requestType)
            {
                case "TaskRequestProxy":
                    TaskRequest t = (TaskRequest)request;
                    TaskRequest updatedT = await _requestQueue.GetTask(t.TaskId);
                    await UpdateTModel(updatedT);
                    RefreshTypingSection(updatedT.History.Status.ToList());
                    SetWaitingToVis(updatedT);
                    break;
                case "IncidentProxy":
                    Incident im = (Incident)request;
                    Incident updatedIM = await _requestQueue.GetIncident(im.IncidentId);
                    await UpdateIMModel(updatedIM);
                    RefreshTypingSection(updatedIM.History.Status.ToList());
                    SetWaitingToVis(updatedIM);
                    break;
                case "ServiceRequestProxy":
                    ServiceRequest c = (ServiceRequest)request;
                    ServiceRequest updatedChange = await _requestQueue.GetChange(c.RequestId);
                    await UpdateCModel(updatedChange);
                    RefreshTypingSection(updatedChange.History.Status.ToList());
                    SetWaitingToVis(updatedChange);
                    break;
            }
        }
        private void SetWaitingToVis(object request)
        {
            if (RequestService.GetStateName(request) == "Waiting")
                nEVM.RequestViewModel.Escalated.WaitingToVis = true;
            else
                nEVM.RequestViewModel.Escalated.WaitingToVis = false;
        }
        private void RefreshTypingSection(List<Status> statuses)
        {
            if (nEVM.RequestViewModel.Escalated != null)
            {
                nEVM.RequestViewModel.Escalated.Statuses = statuses;
                nEVM.RequestViewModel.Escalated.SetStatuses();
            }
        }
        private async Task UpdateIMModel(Incident im)
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

            var contact = await _employeeProfile.GetUIdByIdAsync(im.RequestedPerson);
            nEVM.CUsername = await _userInfo.GetUserNameById(contact);
            nEVM.Contact = null;

            var recipient = await _employeeProfile.GetUIdByIdAsync(im.ContactPerson);
            nEVM.RUsername = await _userInfo.GetUserNameById(recipient);
            nEVM.Recipient = null;

            if (nEVM.RequestViewModel.Escalated != null)
            {
                nEVM.RequestViewModel.Escalated.AUsername = im.Assignee;
                nEVM.RequestViewModel.Escalated.Assignee = null;

                nEVM.RequestViewModel.Escalated.ChoosenGroup =
                    nEVM.RequestViewModel.Escalated.AssigmentGroups
                    .Where(i => i.GroupId == im.GroupId)
                    .FirstOrDefault();
            }

            if (im.History.CloserDue != null)
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
        private async Task UpdateCModel(ServiceRequest c)
        {
            nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests = c;
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

            var contact = await _employeeProfile.GetUIdByIdAsync(c.RequestedPerson);
            nEVM.CUsername = await _userInfo.GetUserNameById(contact);
            nEVM.Contact = null;

            var recipient = await _employeeProfile.GetUIdByIdAsync(c.ContactPerson);
            nEVM.RUsername = await _userInfo.GetUserNameById(recipient);
            nEVM.Recipient = null;

            if (nEVM.RequestViewModel.Escalated != null)
            {
                nEVM.RequestViewModel.Escalated.AUsername = c.Assignee;
                nEVM.RequestViewModel.Escalated.Assignee = null;

                nEVM.RequestViewModel.Escalated.ChoosenGroup =
                    nEVM.RequestViewModel.Escalated.AssigmentGroups
                    .Where(i => i.GroupId == c.GroupId)
                    .FirstOrDefault();
            }

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
        private async Task UpdateTModel(TaskRequest t)
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

            var contact = await _employeeProfile.GetUIdByIdAsync(t.RequestedPerson);
            nEVM.CUsername = await _userInfo.GetUserNameById(contact);
            nEVM.Contact = null;

            var recipient = await _employeeProfile.GetUIdByIdAsync(t.ContactPerson);
            nEVM.RUsername = await _userInfo.GetUserNameById(recipient);
            nEVM.Recipient = null;

            if (nEVM.RequestViewModel.Escalated != null)
            {
                nEVM.RequestViewModel.Escalated.AUsername = t.Assignee;
                nEVM.RequestViewModel.Escalated.Assignee = null;

                nEVM.RequestViewModel.Escalated.ChoosenGroup =
                    nEVM.RequestViewModel.Escalated.AssigmentGroups
                    .Where(i => i.GroupId == t.GroupId)
                    .FirstOrDefault();
            }

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
                        TaskRequest task = await UpdateT("Open");
                        nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests = task;
                        break;
                    case "IncidentProxy":
                        Incident incident = await UpdateIM("Open");
                        nEVM.RequestViewModel.WorkspaceInfo.FirstOrDefault().ServiceRequests = incident;
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest change = await UpdateC("Open");
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
                AdditionalUpdateInfo additional = new AdditionalUpdateInfo();
                additional.Username = nEVM.AdminUsername;
                additional.Phase = "Closed";
                switch (requestType)
                {
                    case "TaskRequestProxy":
                        TaskRequest task = await UpdateT("Closed");
                        await _requestQueue.UpdateTaskRequest(task, additional);
                        break;
                    case "IncidentProxy":
                        Incident incident = await UpdateIM("Closed");
                        await _requestQueue.UpdateIncident(incident, additional);
                        break;
                    case "ServiceRequestProxy":
                        ServiceRequest change = await UpdateC("Closed");
                        await _requestQueue.UpdateServiceRequest(change, additional);
                        break;

                }
                nEVM.RequestViewModel.RemoveCurrentTab();
                nEVM.RequestViewModel.RemoveAssignedRequest(request);
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
        private async Task<Incident> UpdateIM(string statusName)
        {
            string coreNoti = "Request status was set to Closed \n";
            Incident im = RequestService.ConvertRequest(request);
            Incident original = await _requestQueue.GetIncident(im.IncidentId);
            Status newStatus = im.History.ActiveStatus;
            newStatus.State = SetState(statusName);
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

            if (isEscalated)
            {
                im.History.ActiveStatus.StateId = nEVM.CurrentState.StateId;
                im.Assignee = nEVM.RequestViewModel.Escalated.AUsername;
                im.GroupId = nEVM.RequestViewModel.Escalated.ChoosenGroup.GroupId;
                im.Group = nEVM.RequestViewModel.Escalated.ChoosenGroup;
                coreNoti = "Request status was set to " + nEVM.CurrentState.StateName + "\n";
                string contactId = await _employeeProfile.GetUIdByIdAsync(original.RequestedPerson);
                string recipientId = await _employeeProfile.GetUIdByIdAsync(original.ContactPerson);
                usernames.Add(await _userInfo.GetUserNameById(recipientId));
                usernames.Add(await _userInfo.GetUserNameById(contactId));
            }
            else if (!isEscalated && statusName == "Open")
                coreNoti = "New request has been registered and escalted to ";

            if (!(!isEscalated && statusName == "Open"))
            {
                usernames.Add(nEVM.RUsername);
                usernames.Add(nEVM.CUsername);
                var changes = RequestService.FindChanges(original, im, usernames);
                Notification notification = new Notification();
                im.History.ActiveStatus.Notification = coreNoti + notification.NotificationBuilder(changes);
            }

            hasGroupChanged = String.Compare(original.GroupId.ToString(), im.GroupId.ToString());

            return im;
        }
        private async Task<TaskRequest> UpdateT(string statusName)
        {
            string coreNoti = "Request status was set to Closed \n";
            TaskRequest t = RequestService.ConvertRequest(request);
            TaskRequest original = await _requestQueue.GetTask(t.TaskId);
            Status newStatus = t.History.ActiveStatus;
            newStatus.State = SetState(statusName);
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

            if (isEscalated)
            {
                t.History.ActiveStatus.StateId = nEVM.CurrentState.StateId;
                t.Assignee = nEVM.RequestViewModel.Escalated.AUsername;
                t.GroupId = nEVM.RequestViewModel.Escalated.ChoosenGroup.GroupId;
                t.Group = nEVM.RequestViewModel.Escalated.ChoosenGroup;
                coreNoti = "Request status was set to " + nEVM.CurrentState.StateName + "\n";
                string contactId = await _employeeProfile.GetUIdByIdAsync(original.RequestedPerson);
                string recipientId = await _employeeProfile.GetUIdByIdAsync(original.ContactPerson);
                usernames.Add(await _userInfo.GetUserNameById(recipientId));
                usernames.Add(await _userInfo.GetUserNameById(contactId));
            }
            else if (!isEscalated && statusName == "Open")
                coreNoti = "New request has been registered and escalted to ";

            if (!(!isEscalated && statusName == "Open"))
            {
                usernames.Add(nEVM.RUsername);
                usernames.Add(nEVM.CUsername);
                var changes = RequestService.FindChanges(original, t, usernames);
                Notification notification = new Notification();
                t.History.ActiveStatus.Notification = coreNoti + notification.NotificationBuilder(changes);
            }

            hasGroupChanged = String.Compare(original.GroupId.ToString(), t.GroupId.ToString());

            return t;
        }
        private async Task<ServiceRequest> UpdateC(string statusName)
        {
            string coreNoti = "Request status was set to Closed \n";
            ServiceRequest c = RequestService.ConvertRequest(request);
            ServiceRequest original = await _requestQueue.GetChange(c.RequestId);
            Status newStatus = c.History.ActiveStatus;
            newStatus.State = SetState(statusName);
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

            if (isEscalated)
            {
                c.History.ActiveStatus.StateId = nEVM.CurrentState.StateId;
                c.Assignee = nEVM.RequestViewModel.Escalated.AUsername;
                c.GroupId = nEVM.RequestViewModel.Escalated.ChoosenGroup.GroupId;
                c.Group = nEVM.RequestViewModel.Escalated.ChoosenGroup;
                coreNoti = "Request status was set to " + nEVM.CurrentState.StateName + "\n";
                string contactId = await _employeeProfile.GetUIdByIdAsync(original.RequestedPerson);
                string recipientId = await _employeeProfile.GetUIdByIdAsync(original.ContactPerson);
                usernames.Add(await _userInfo.GetUserNameById(recipientId));
                usernames.Add(await _userInfo.GetUserNameById(contactId));
            }
            else if (!isEscalated && statusName == "Open")
                coreNoti = "New request has been registered and escalted to ";

            if (!(!isEscalated && statusName == "Open"))
            {
                var changes = RequestService.FindChanges(original, c, usernames);
                Notification notification = new Notification();
                c.History.ActiveStatus.Notification = coreNoti + notification.NotificationBuilder(changes);
            }

            hasGroupChanged = String.Compare(original.GroupId.ToString(), c.GroupId.ToString());

            return c;
        }
        private async Task<int> GetEmployeeId(string userName)
        {
            string userId = await _userInfo.GetUserId(userName);
            return await _employeeProfile.GetEmployeeIdByUId(userId);
        }
        private State SetState(string statusName)
        {
            if (statusName == "Closed")
                return nEVM.States.Where(n => n.StateName == "Closed").FirstOrDefault();
            else if (statusName == "Open")
                return nEVM.States.Where(n => n.StateName == "Open").FirstOrDefault();
            else
                return nEVM.States.Where(n => n.StateName == "Waiting").FirstOrDefault();
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