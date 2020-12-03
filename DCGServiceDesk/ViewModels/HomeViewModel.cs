using DCGServiceDesk.Session.CurrentUser;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        public ILoggedUser loggedUser;
        public HomeViewModel(ILoggedUser loggedUser)
        {
            this.loggedUser = loggedUser;
        }

    }
}
