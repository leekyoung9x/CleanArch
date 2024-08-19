using CleanArch.Core.Entities.PagingData;
using System.Data;

namespace CleanArch.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<PagingData<T>> GetPaging(string sql, object parameters = null, CommandType commandType = CommandType.Text);
        Task<T?> GetByIdAsync(long id);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(long id);
    }
}
