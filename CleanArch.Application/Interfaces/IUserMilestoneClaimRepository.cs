using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IUserMilestoneClaimRepository : IRepository<UserMilestoneClaim>
    {
        Task<IEnumerable<UserMilestoneClaim>> GetClaimsByUserIdAsync(long userId);
        Task<bool> HasUserClaimedMilestoneAsync(long userId, int milestoneId);
        Task<IEnumerable<UserMilestoneClaim>> GetRecentClaimsAsync(int limit = 10);
    }
}
