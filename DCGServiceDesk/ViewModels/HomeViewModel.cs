using DCGServiceDesk.Commands;
using DCGServiceDesk.Session.CurrentUser;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.Session.Queue;
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
        private readonly IQueueCreator _queueCreator;
        public HomeViewModel(ILoggedUser loggedUser, IRequestQueue requestQueue, IQueueCreator queueCreator)
        {
            this.loggedUser = loggedUser;
            _requestQueue = requestQueue;
            _queueCreator = queueCreator;

            RequestCommand = new RequestCommand(_requestQueue, _queueCreator);
        }

    }
}
