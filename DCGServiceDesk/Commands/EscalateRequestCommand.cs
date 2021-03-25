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
        private readonly HomeViewModel _hVM;
        private Tab tab;
        public EscalateRequestCommand(Tab tab, HomeViewModel homeViewModel, IRequestQueue requestQueue)
        {
            _hVM = homeViewModel;
            _requestQueue = requestQueue;
            this.tab = tab;
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
