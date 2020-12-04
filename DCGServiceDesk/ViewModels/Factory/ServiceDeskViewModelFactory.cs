using System;
using System.Collections.Generic;
using System.Text;
using DCGServiceDesk.Session.Navigation;
using GalaSoft.MvvmLight;

namespace DCGServiceDesk.ViewModels.Factory
{
    class ServiceDeskViewModelFactory : IServiceDeskViewModelFactory
    {
        private readonly CreateViewModel<LoginViewModel> _createLoginViewModel;
        private readonly CreateViewModel<HomeViewModel> _createHomeViewModel;

        public ServiceDeskViewModelFactory(CreateViewModel<LoginViewModel> createLoginViewModel,
            CreateViewModel<HomeViewModel> createHomeViewModel)
        {
            _createLoginViewModel = createLoginViewModel;
            _createHomeViewModel = createHomeViewModel;
        }

        public ViewModelBase CreateViewModel(ViewName viewName)
        {
            switch (viewName)
            {
                case ViewName.Login:
                    return _createLoginViewModel();
                case ViewName.MainView:
                    return _createHomeViewModel();
                default:
                    throw new ArgumentException("The ViewName does not have a ViewModel.", "viewName");
            }
        }
    }
}
