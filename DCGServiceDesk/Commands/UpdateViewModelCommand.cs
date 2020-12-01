using DCGServiceDesk.Session.Navigation;
using DCGServiceDesk.ViewModels.Factory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.Commands
{
    public class UpdateViewModelCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IViewForwarding _forwarding;
        private readonly IServiceDeskViewModelFactory _viewModelFactory;

        public UpdateViewModelCommand(IViewForwarding forwarding, IServiceDeskViewModelFactory viewModelFactory)
        {
            _forwarding = forwarding;
            _viewModelFactory = viewModelFactory;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(parameter is ViewName)
            {
                ViewName viewName = (ViewName)parameter;

                _forwarding.ActiveViewModel = _viewModelFactory.CreateViewModel(viewName);
            }
        }
    }
}
