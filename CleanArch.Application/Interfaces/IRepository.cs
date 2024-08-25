using System.Data;

namespace CleanArch.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(long id);

        Task<IEnumerable<T>> GetByIdAsync(string column, object Id);

        Task<bool> AddAsync(T entity);

        Task<bool> AddAsync(T entity, IDbConnection connection, IDbTransaction transaction);

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(long id);

        Task<bool> UpdateAsync(T entity, IDbConnection connection, IDbTransaction transaction);
    }
}