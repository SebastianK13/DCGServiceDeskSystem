using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.EF.Services
{
    public class AdditionalUpdateInfo
    {
        public AdditionalUpdateInfo()
        {
            Phase = "Open";
        }
        public string Username { get; set; }
        public string Phase { get; set; }
        public string Notification { get; set; }
    }
}
