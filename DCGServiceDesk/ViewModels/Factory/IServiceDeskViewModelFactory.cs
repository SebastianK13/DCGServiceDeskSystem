using DCGServiceDesk.Session.Navigation;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.ViewModels.Factory
{
    public interface IServiceDeskViewModelFactory
    {
        ViewModelBase CreateViewModel(ViewName viewName);
    }
}
