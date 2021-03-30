using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class EscalateRequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly RequestViewModel _rVM;
        public EscalateRequestCommand(RequestViewModel rVM, IRequestQueue requestQueue)
        {
            _requestQueue = requestQueue;
            _rVM = rVM;
        }
        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                string option = parameter.GetType().Name;
                if (option == "string")
                    option = parameter.ToString();

                switch (option)
                {
                    case "NotEscalatedViewModel":
                        NotEscalatedViewModel notEscalated = ((NotEscalatedViewModel)parameter);
                        notEscalated.RequestViewModel.CurrentMode = notEscalated.RequestViewModel.Escalation;
                        notEscalated.RequestViewModel.Escalation.RequestViewModel = notEscalated.RequestViewModel;
                        break;
                    case "EscalationViewModel":
                        
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
