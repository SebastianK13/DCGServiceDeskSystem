﻿using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    public interface IRequestQueue
    {
        Task<List<ServiceRequest>> GetRequests();
    }
}