using System;
using System.Collections.Generic;
using System.Text;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.Session.CurrentUser
{
    public interface ILoggedUser
    {
        string ActiveUser { get; set; }
    }
}
