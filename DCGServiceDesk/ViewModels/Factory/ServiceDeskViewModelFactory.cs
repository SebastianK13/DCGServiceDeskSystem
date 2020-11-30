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

        public ServiceDeskViewModelFactory(CreateViewModel<LoginViewModel> createLoginViewModel)
        {
            _createLoginViewModel = createLoginViewModel;
        }

        public ViewModelBase CreateViewModel(ViewName viewName)
        {
            switch (viewName)
            {
                case ViewName.Login:
                    return _createLoginViewModel();
                default:
                    throw new ArgumentException("The ViewName does not have a ViewModel.", "viewName");
            }
        }
    }
}
