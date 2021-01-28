using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class RequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        
        public RequestCommand(IRequestQueue requestQueue)
        {
            _requestQueue = requestQueue;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
               var t = await _requestQueue.GetRequests();

            }
            catch (Exception e)
            {

            }
        }
    }
}
