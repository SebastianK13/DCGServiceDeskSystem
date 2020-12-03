using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.Navigation;
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

        public ICommand LoginCommand { get; }
        public ICommand UpdateViewModelCommand { get; }

        public LoginViewModel(IAuthorization authenticator)
        {

            LoginCommand = new LoginCommand(this, authenticator);
            UpdateViewModelCommand.Execute(ViewName.MainView);
        }
    }
}
