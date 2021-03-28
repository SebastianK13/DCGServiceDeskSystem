using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    public interface IUserInfo
    {
        Task<List<CommunicationInfo>> GetUserName(List<string> eIdContact, List<string> eIdRequested);
        Task<string> GetUserId(string username);
        Task<string> GetUserNameById(string userId);
    }
}
