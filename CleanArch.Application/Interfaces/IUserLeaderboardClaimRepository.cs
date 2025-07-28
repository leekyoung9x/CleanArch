using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IUserLeaderboardClaimRepository : IRepository<UserLeaderboardClaim>
    {
        Task<IEnumerable<UserLeaderboardClaim>> GetClaimsByUserIdAsync(long userId);
        Task<UserLeaderboardClaim> GetClaimByUserAndSeasonAsync(long userId, int seasonId);
        Task<bool> HasUserClaimedSeasonAsync(long userId, int seasonId);
        Task<IEnumerable<UserLeaderboardClaim>> GetClaimsBySeasonIdAsync(int seasonId);
    }
}
