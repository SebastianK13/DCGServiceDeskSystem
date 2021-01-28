using DCGServiceDesk.Commands;
using DCGServiceDesk.Session.CurrentUser;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DCGServiceDesk.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ILoggedUser loggedUser;
        public ICommand RequestCommand { get; }
        private readonly IRequestQueue _requestQueue;
        public HomeViewModel(ILoggedUser loggedUser, IRequestQueue requestQueue)
        {
            this.loggedUser = loggedUser;
            _requestQueue = requestQueue;

            RequestCommand = new RequestCommand(_requestQueue);
        }

    }
}
