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
            NotEscalated = new NotEscalatedViewModel(RequestService.ConvertRequest(singleRequest.Request));
            NotEscalated.RequestViewModel = this;
            Escalation = new EscalationViewModel() { AssigmentGroups = singleRequest.Groups };
            CurrentMode = NotEscalated;
        }
    }
    public class NotEscalatedViewModel:ViewModelBase
    {
        public RequestViewModel RequestViewModel { get; set; }
        public NotEscalatedViewModel(TaskRequest t)
        {
            CurrentState = t.Status.State;
        }
        public NotEscalatedViewModel(Incident i)
        {
            CurrentState = i.Status.State;
        }
        public NotEscalatedViewModel(ServiceRequest c)
        {
            CurrentState = c.Status.State;
        }
        private State _state;
        public State CurrentState
        {
            get { return _state; }
            set
            {
                if (value != null)
                {
                    _state = value;
                    OnPropertyChanged("CurrentState");
                }
            }
        } 

    }
    public class EscalationViewModel
    {
        public List<AssigmentGroup> AssigmentGroups { get; set; }
        public RequestViewModel RequestViewModel { get; set; }
    }
}
