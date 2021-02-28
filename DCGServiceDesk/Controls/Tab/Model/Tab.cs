using DCGServiceDesk.Commands;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DCGServiceDesk.Controls.Tab.Model
{
    public abstract class Tab : ITab
    {
        public Tab()
        {
            CloseTabCommand = new ActionCommand(p => CloseTabRequested?.Invoke(this, EventArgs.Empty));
        }
        public List<TabContainer> WorkspaceInfo { get; set; } = new List<TabContainer>();
        public string Label { get; set; }
        public ICommand CloseTabCommand { get; }
        public object CloseRequested { get; }
        public event EventHandler CloseTabRequested;
    }
    public class TabContainer
    {
        public object ServiceRequests { get; set; } = new List<object>();
        public CommunicationInfo CommunicationInfo { get; set; }
        public bool RequestVisibility { get; set; }
        public string RequestType { get; set; }
    }
}
