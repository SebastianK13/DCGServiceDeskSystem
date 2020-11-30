using DCGServiceDesk.Data.Services;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{

    public class LoginCommand : AsyncCommandBase
    {
        private readonly LoginViewModel _loginViewModel;
        private readonly IAuthorization _authorization;


        public LoginCommand(LoginViewModel loginViewModel, IAuthorization authorization)
        {
            _loginViewModel = loginViewModel;
            _authorization = authorization;
        }

        public override async Task ExecuteAsync(object parameter)
        {

            try
            {
                await _authorization.Login(_loginViewModel.Username, _loginViewModel.Password);
            }
            catch (Exception)
            {

            }
        }
    }
}
