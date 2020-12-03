using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.Session.Navigation
{
    public enum ViewName
    {
        Login,
        MainView
    }
    public interface IViewForwarding
    {
        ViewModelBase ActiveViewModel { get; set; }
        event Action StateChanged;
    }
}
