using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCGServiceDesk.Commands
{
    public class EscalateRequestCommand : ICommand
    {
        private readonly IRequestQueue _requestQueue;
        private readonly RequestViewModel _rVM;
        public EscalateRequestCommand(RequestViewModel rVM, IRequestQueue requestQueue)
        {
            _requestQueue = requestQueue;
            _rVM = rVM;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            try
            {
                string option = parameter.ToString();

                switch (option)
                {
                    case "Escalation":
                        _rVM.CurrentMode = _rVM.Escalation;
                        break;
                    case "NotEscalated":
                        _rVM.CurrentMode = _rVM.NotEscalated;
                        break;
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
