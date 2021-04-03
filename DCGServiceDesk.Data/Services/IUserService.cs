using System.Collections.Generic;
using System.Threading.Tasks;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.Data.Services
{
    public interface IUserService
    {
        Task<User> GetByName(string username);
        Task<List<CommunicationInfo>> GetUserName(List<string> eIdContact, List<string> eIdRequested);
        Task<string> GetUserId(string username);
        Task<string> GetUserNameById(string userId);
    }
}