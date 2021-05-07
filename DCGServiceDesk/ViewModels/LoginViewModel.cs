using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
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
        private bool _errorVis;
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
        public bool LoginErrorVisibility
        {
            get { return _errorVis; }
            set
            {
                _errorVis = value;
                OnPropertyChanged("LoginErrorVisibility");
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand UpdateViewModelCommand { get; }

        public LoginViewModel(IViewForwarding forwarding, 
            IServiceDeskViewModelFactory viewModelFactory, LoginInterfaceContainer interfaceContainer)
        {
            _forwarding = forwarding;
            _viewModelFactory = viewModelFactory;
            _authorization = interfaceContainer.Authorization;

            LoginCommand = new LoginCommand(this, interfaceContainer, forwarding, viewModelFactory);
        }

    }
    public class LoginInterfaceContainer
    {
        public IAuthorization Authorization { get; set; }
        public IEmployeeProfile EmployeeProfile { get; set; }
    }
}
