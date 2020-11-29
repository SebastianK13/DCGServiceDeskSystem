using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Services
{
    public interface ICrud<T>
    {
        Task<List<T>> GetAll();
        Task<T> Get(int id);
        Task<T> Update(int id, T entity);
        Task<bool> Remove(int id);
    }
}