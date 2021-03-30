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
        private NotEscalatedViewModel _nEVM;
        private Employee _user;
        private string superiorUsername;

        public FindUserCommand(DbInterfaceContainer interfaces, NotEscalatedViewModel nEVM)
        {
            _userInfo = interfaces.UserInfo;
            _employeeProfile = interfaces.EmployeeProfile;
            _nEVM = nEVM;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            string id = "";
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
                    break;
                case "FindContact":
                     id = await _userInfo.GetUserId(_nEVM.CUsername);
                    _nEVM.ContactValid = await GenerateProperColorAsync(id);
                    if(_user != null)
                    {
                        _nEVM.Contact = new AccountInfo(_user, superiorUsername);
                        _nEVM.FindUserEventArea = true;
                    }
                    break;
                case "CloseUserInfo":
                    CloseOpened();
                    _nEVM.FindUserEventArea = false;
                    break;
            }
        }

        private async Task<SolidColorBrush> GenerateProperColorAsync(string id)
        {
            if (id != "" && id != null)
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
        private void CloseOpened()
        {
            if (_nEVM.Contact != null)
                _nEVM.Contact = null;
            else if (_nEVM.Recipient != null)
                _nEVM.Recipient = null;
        }
            
    }
}
