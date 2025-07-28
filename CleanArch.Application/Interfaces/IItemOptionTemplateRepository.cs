using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IItemOptionTemplateRepository : IRepository<ItemOptionTemplate>
    {
        Task<IEnumerable<ItemOptionTemplate>> GetByTypeAsync(int type);
        Task<IEnumerable<ItemOptionTemplate>> GetByIdsAsync(IEnumerable<int> ids);
    }
}
