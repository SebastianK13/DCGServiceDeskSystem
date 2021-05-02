using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;

namespace DCGServiceDesk.Commands
{
    public class FindUserCommand : AsyncCommandBase
    {
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private readonly IRequestQueue _requestQueue;
        private NotEscalatedViewModel _nEVM;
        private Employee _user;
        private string superiorUsername;

        public FindUserCommand(DbInterfaceContainer interfaces, NotEscalatedViewModel nEVM)
        {
            _requestQueue = interfaces.RequestQueue;
            _userInfo = interfaces.UserInfo;
            _employeeProfile = interfaces.EmployeeProfile;
            _nEVM = nEVM;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            string id = "";
            _user = null;
            switch (parameter.ToString())
            {
                case "FindRecipient":
                     id = await _userInfo.GetUserId(_nEVM.RUsername);
                    _nEVM.RecipientValid = await GenerateProperColorAsync(id);
                    if(_user != null)
                    {
                        _nEVM.Recipient = new AccountInfo(_user, superiorUsername);
                        _nEVM.FindUserEventArea = true;
                    }
                    else
                        _nEVM.Recipient = null;
                    break;
                case "FindContact":
                     id = await _userInfo.GetUserId(_nEVM.CUsername);
                    _nEVM.ContactValid = await GenerateProperColorAsync(id);
                    if (_user != null)
                    {
                        _nEVM.Contact = new AccountInfo(_user, superiorUsername);
                        _nEVM.FindUserEventArea = true;
                    }
                    else
                        _nEVM.Contact = null;
                    break;
                case "CloseUserInfo":
                    CloseOpened();
                    _nEVM.FindUserEventArea = false;
                    break;
                case "CloseAssigneeInfo":
                    CloseAssigneeOpened();
                    _nEVM.RequestViewModel.Escalated.FindAssigneeEventArea = false;
                    break;
                case "FindAssignee":
                    id = await _userInfo.GetUserId(_nEVM.RequestViewModel.Escalated.AUsername);
                    _nEVM.RequestViewModel.Escalated.AssigneeValid = 
                        await GenerateAssigneeColor(id, 
                        await CheckIsMember(_nEVM.RequestViewModel.Escalated.AUsername));
                    if (_user != null)
                    {
                        _nEVM.RequestViewModel.Escalated.Assignee = new AccountInfo(_user, superiorUsername);
                        _nEVM.RequestViewModel.Escalated.FindAssigneeEventArea = true;
                    }
                    else
                        _nEVM.RequestViewModel.Escalated.Assignee = null;
                    break;
                case "ShowMembers":
                    await CreateMembersList();
                    _nEVM.RequestViewModel.Escalated.MembersVisibility = true;
                    break;
                case "HideMembers":
                    _nEVM.RequestViewModel.Escalated.MembersVisibility = false;
                    break;
            }
        }
        private async Task CreateMembersList()
        {
            List<MemberInfo> membersInfo = new List<MemberInfo>();
            List<string> members = await _requestQueue
                .GetMembers(_nEVM.RequestViewModel.Escalated.ChoosenGroup.GroupId);

            foreach(var i in members)
            {
                string id = await _userInfo.GetUserId(i);
                Employee e = await _employeeProfile.GetUser(id);
                membersInfo.Add(new MemberInfo { 
                     Username = i,
                     Firstname = e.Firstname,
                     Surname = e.Surname
                });
            }

            _nEVM.RequestViewModel.Escalated.Members = membersInfo;
        }

        private async Task<bool> CheckIsMember(string username) =>
            await _requestQueue.IsGroupMember(username, _nEVM.RequestViewModel.Escalated.ChoosenGroup.GroupId);
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
        private void CloseAssigneeOpened()
        {
            if (_nEVM.RequestViewModel.Escalated.Assignee != null)
                _nEVM.RequestViewModel.Escalated.Assignee = null;
        }
        private void CloseOpened()
        {
            if (_nEVM.Contact != null)
                _nEVM.Contact = null;
            else if (_nEVM.Recipient != null)
                _nEVM.Recipient = null;
        }
            
    }
}
