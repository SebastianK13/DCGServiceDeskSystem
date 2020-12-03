using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.EF.Context;
using DCGServiceDesk.EF.Factory;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.Navigation;
using DCGServiceDesk.ViewModels.Factory;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Unity;

namespace DCGServiceDesk.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {
        private readonly IViewForwarding _forwarding;
        private readonly IServiceDeskViewModelFactory _viewModelFactory;
        private readonly IAuthorization _authorization;

        public bool IsLogged => _authorization.IsLogged;

        public ViewModelBase ActiveViewModel => _forwarding.ActiveViewModel;

        public ICommand UpdateViewModelCommand { get; }

        public MainWindowViewModel(IViewForwarding forwarding, IServiceDeskViewModelFactory viewModelFactory, IAuthorization authorization) 
        {
            _forwarding = forwarding;
            _viewModelFactory = viewModelFactory;
            _authorization = authorization;

            _forwarding.StateChanged += Forwarding;

            UpdateViewModelCommand = new UpdateViewModelCommand(forwarding, _viewModelFactory);
            UpdateViewModelCommand.Execute(ViewName.Login);
        }

        private void Forwarding()
        {
            OnPropertyChanged(nameof(ActiveViewModel));
        }
    }
}



