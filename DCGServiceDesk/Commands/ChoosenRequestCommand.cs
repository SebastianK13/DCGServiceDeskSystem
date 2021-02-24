using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class ChoosenRequestCommand : AsyncCommandBase
    {
        private HomeViewModel _hVM;
        private readonly IRequestQueue _requestQueue;
        public ChoosenRequestCommand(HomeViewModel hVM)
        {
            _hVM = hVM;
        }
        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                string choosen = parameter as string;
                switch (parameter.GetType().Name)
                {
                    //case "Incident":
                    //    _hVM.Tabs.Add(new RequestViewModel((Incident)parameter, "Incident"));
                    //    break;
                    //case "ServiceRequest":
                    //    _hVM.Tabs.Add(new RequestViewModel((ServiceRequest)parameter, "Change"));
                    //    break;
                    //case "TaskRequest":
                    //    _hVM.Tabs.Add(new RequestViewModel((TaskRequest)parameter, "Task"));
                    //    break;
                    //default:
                    //    break;
                }

            }
            catch (Exception e)
            {

            }
        }
    }
}
