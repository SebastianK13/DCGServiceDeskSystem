using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class RequestViewModel:Tab
    {
        public List<State> States { get; set; }
        public RequestViewModel(SingleRequestInfo singleRequest, 
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
