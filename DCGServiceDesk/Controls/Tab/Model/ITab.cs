using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.Controls.Tab.Model
{
    public interface ITab
    {
        string Label { get; set; }
        List<object> ServiceRequests { get; set; }
        ICommand CloseTabCommand { get; }
        event EventHandler CloseTabRequested;
    }
}
