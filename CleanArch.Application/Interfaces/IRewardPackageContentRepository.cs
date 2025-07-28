using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IRewardPackageContentRepository : IRepository<RewardPackageContent>
    {
        Task<IEnumerable<RewardPackageContent>> GetContentsByPackageIdAsync(int packageId);
        Task<bool> DeleteByPackageIdAsync(int packageId);
    }
}
