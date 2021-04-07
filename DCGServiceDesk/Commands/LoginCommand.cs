using DCGServiceDesk.Data.Services;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.Session.Navigation;
using DCGServiceDesk.ViewModels;
using DCGServiceDesk.ViewModels.Factory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DCGServiceDesk.Commands
{

    public class LoginCommand : AsyncCommandBase
    {
        public ICommand UpdateViewModelCommand { get; }
        private readonly LoginViewModel _loginViewModel;
        private readonly IAuthorization _authorization;
        private readonly IEmployeeProfile _employeeProfile;
        private readonly IViewForwarding _forwarding;
        private readonly IServiceDeskViewModelFactory _viewModelFactory;

        public LoginCommand(LoginViewModel loginViewModel, LoginInterfaceContainer interfaceContainer, IViewForwarding forwarding,
            IServiceDeskViewModelFactory viewModelFactory)
        {
            _loginViewModel = loginViewModel;
            _authorization = interfaceContainer.Authorization;
            _employeeProfile = interfaceContainer.EmployeeProfile;
            _forwarding = forwarding;
            _viewModelFactory = viewModelFactory;
            UpdateViewModelCommand = new UpdateViewModelCommand(_forwarding, _viewModelFactory);
        }

        public override async Task ExecuteAsync(object parameter)
        {

            try
            {
                PasswordBox pswBox = parameter as PasswordBox;
                var result = await _authorization.Login(_loginViewModel.Username, pswBox.Password);
                if(result != null)
                {
                    await _authorization.SetTimeZone(_employeeProfile);
                    UpdateViewModelCommand.Execute(ViewName.MainView);
                }

            }
            catch (Exception e)
            {

            }
        }
    }
}
