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
        public EscalateRequestCommand(HomeViewModel homeViewModel, IRequestQueue requestQueue)
        {
            _hVM = homeViewModel;
            _requestQueue = requestQueue;
        }
        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                string option = parameter.GetType().Name;

                switch (option)
                {
                    case "IncidentProxy":
                        //method implementation
                        break;
                    case "ServiceRequestProxy":
                        //method implementation
                        break;
                    case "TaskRequestProxy":
                        //method implementation
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
