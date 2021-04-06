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
        private readonly HomeViewModel _hVM;
        private readonly IRequestQueue _requestQueue;

        public UpdateGroupsCommand(HomeViewModel homeViewModel, IRequestQueue requestQueue)
        {
            _hVM = homeViewModel;
            _requestQueue = requestQueue;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                _hVM.Groups = await _requestQueue.GetUserGroups(_hVM.loggedUser.ActiveUser);
            }
            catch(Exception e)
            {

            }
        }
    }
}
