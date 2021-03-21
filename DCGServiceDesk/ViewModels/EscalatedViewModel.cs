using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.ViewModels
{
    public class EscalatedViewModel:Tab
    {
        public List<State> States { get; set; }
        public EscalatedViewModel(SingleRequestInfo singleRequest,
            DbInterfaceContainer interfaceContainer, HomeViewModel hVM) 
            : base(interfaceContainer, hVM)
        {
            States = singleRequest.States;
            TabContainer wi = new TabContainer();
            wi.ServiceRequests = singleRequest.Request;
            wi.CommunicationInfo = singleRequest.Info;
            wi.RequestVisibility = true;
            WorkspaceInfo = new List<TabContainer> { wi };
            Label = singleRequest.Label;
        }
    }
}
