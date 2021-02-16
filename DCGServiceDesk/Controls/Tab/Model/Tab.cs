using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DCGServiceDesk.Controls.Tab.Model
{
    public abstract class Tab:ITab
    {
        public Tab()
        {
            CloseTabCommand = new ActionCommand(p => CloseTabRequested?.Invoke(this, EventArgs.Empty));
        }
        public string Label { get; set; }
        public List<object> ServiceRequests { get; set; } = new List<object>();
        public ICommand CloseTabCommand { get; }
        public object CloseRequested { get; }
        public event EventHandler CloseTabRequested;
    }
}
