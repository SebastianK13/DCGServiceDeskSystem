using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.Navigation;
using DCGServiceDesk.ViewModels.Factory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username = "Noname";
        private readonly IViewForwarding _forwarding;
        private readonly IServiceDeskViewModelFactory _viewModelFactory;
        private readonly IAuthorization _authorization;
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

        public LoginViewModel(IViewForwarding forwarding, IServiceDeskViewModelFactory viewModelFactory, IAuthorization authenticator)
        {
            _forwarding = forwarding;
            _viewModelFactory = viewModelFactory;
            _authorization = authenticator;

            LoginCommand = new LoginCommand(this, _authorization, forwarding, viewModelFactory);
        }

    }
}
