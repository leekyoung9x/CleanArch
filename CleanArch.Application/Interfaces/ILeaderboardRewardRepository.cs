using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface ILeaderboardRewardRepository : IRepository<LeaderboardReward>
    {
        Task<IEnumerable<LeaderboardReward>> GetRewardsBySeasonIdAsync(int seasonId);
        Task<LeaderboardReward> GetRewardByRankAsync(int seasonId, int rank);
        Task<IEnumerable<LeaderboardReward>> GetAllWithPackagesAsync();
    }
}
