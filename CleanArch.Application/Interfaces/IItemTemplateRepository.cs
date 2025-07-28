using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IItemTemplateRepository : IRepository<ItemTemplate>
    {
        Task<ItemTemplate> GetByIdWithOptionsAsync(int id);
        Task<IEnumerable<ItemTemplate>> GetByIdsWithOptionsAsync(IEnumerable<int> ids);
    }
}
