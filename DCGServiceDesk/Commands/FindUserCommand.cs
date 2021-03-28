using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public FindUserCommand(DbInterfaceContainer interfaces, NotEscalatedViewModel nEVM)
        {
            _userInfo = interfaces.UserInfo;
            _employeeProfile = interfaces.EmployeeProfile;
            _nEVM = nEVM;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            await _userInfo.GetUserId((string)parameter);
            _nEVM.Recipient = new AccountInfo();
        }

    }
}
