using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Application.Interfaces
{
    public interface IMilestoneRewardRepository : IRepository<MilestoneReward>
    {
        Task<IEnumerable<MilestoneReward>> GetMilestonesByScoreAsync(long userScore);
        Task<MilestoneReward> GetByRequiredScoreAsync(long requiredScore);
        Task<IEnumerable<MilestoneReward>> GetAllWithPackagesAsync();
        Task<IEnumerable<MilestoneRewardResponse>> GetMilestoneRewardsForClientAsync(long userId, long userScore);
        Task<IEnumerable<MilestoneRewardResponse>> GetAllMilestoneRewardsForClientAsync(long userId, long userScore);
    }
}
