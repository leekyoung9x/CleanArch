using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IRewardPackageRepository : IRepository<RewardPackage>
    {
        Task<RewardPackage> GetByIdWithContentsAsync(int id);
        Task<IEnumerable<RewardPackage>> GetAllWithContentsAsync();
    }
}
