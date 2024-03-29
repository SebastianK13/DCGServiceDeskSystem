﻿using System;
using System.Collections.Generic;
using System.Text;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.Session.CurrentUser
{
    public class LoggedUser : ILoggedUser
    {
        public string ActiveUser { get; set; }
        public string UserID { get; set; }
        public TimeZoneInfo ZoneInfo { get; set; }
    }
}
