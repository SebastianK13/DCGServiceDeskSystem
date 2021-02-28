using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;

namespace DCGServiceDesk.Commands
{
    public class UpdateGroupsCommand : AsyncCommandBase
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly IRequestQueue _requestQueue;

        public UpdateGroupsCommand(HomeViewModel homeViewModel, IRequestQueue requestQueue)
        {
            _homeViewModel = homeViewModel;
            _requestQueue = requestQueue;
        }

        public override Task ExecuteAsync(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
