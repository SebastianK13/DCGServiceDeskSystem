using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username = "Noname";
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthorization authenticator)
        {

            LoginCommand = new LoginCommand(this, authenticator);
        }
    }
}
