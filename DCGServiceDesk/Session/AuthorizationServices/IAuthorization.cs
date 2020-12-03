using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Services
{
    public interface IAuthorization
    {
        bool IsLogged { get;}

        Task<User> Login(string username, string password);
    }
}