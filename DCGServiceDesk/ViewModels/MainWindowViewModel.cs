using DCGServiceDesk.Data.Services;
using DCGServiceDesk.EF.Context;
using DCGServiceDesk.EF.Factory;
using DCGServiceDesk.Session.Navigation;
using DCGServiceDesk.ViewModels.Factory;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Unity;

namespace DCGServiceDesk.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {
        private readonly IDatabaseContextFactory _databaseContextFactor;
        private readonly IViewForwarding _forwarding;
        private readonly IServiceDeskViewModelFactory _viewModelFactory;
        private readonly IAuthorization _authorization;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase ActiveViewModel => _forwarding.ActiveViewModel;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainWindowViewModel(IViewForwarding forwarding, IServiceDeskViewModelFactory viewModelFactory, IAuthorization authorization) 
        {
            _forwarding = forwarding;
            _viewModelFactory = viewModelFactory;
            _authorization = authorization;

            _forwarding.StateChanged += Forwarding;
            
            //UpdateViewModelCommand = new ToDo
        }

        private void Forwarding()
        {
            OnPropertyChanged(nameof(ActiveViewModel));
        }

        //public MainWindowViewModel(IDatabaseContextFactory databaseContextFactory)
        //{
        //    _databaseContextFactor = databaseContextFactory;
        //}


    }
}



