using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;

namespace DCGServiceDesk.Session.DataGetter
{
    class RequestQueue : IRequestQueue
    {
        private readonly IRequestService _requestService;
        public RequestQueue(IRequestService requestService)
        {
            _requestService = requestService;
        }
        public async Task<List<ServiceRequest>> GetRequests()
        {
            List<ServiceRequest> requests = new List<ServiceRequest>();

            return await _requestService.GetAll();
        }
    }
}
