using System.Threading.Tasks;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.Data.Services
{
    public interface IUserService : ICrud<User>
    {
        Task<User> GetByName(string username);
    }
}