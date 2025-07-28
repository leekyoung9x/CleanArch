using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface ILeaderboardSeasonRepository : IRepository<LeaderboardSeason>
    {
        Task<LeaderboardSeason> GetActiveSeasonAsync();
        Task<IEnumerable<LeaderboardSeason>> GetSeasonsByStatusAsync(string status);
        Task<LeaderboardSeason> GetSeasonWithRewardsAsync(int seasonId);
    }
}
