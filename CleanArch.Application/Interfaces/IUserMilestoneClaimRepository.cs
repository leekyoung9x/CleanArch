using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Application.Interfaces
{
    public interface IUserMilestoneClaimRepository : IRepository<UserMilestoneClaim>
    {
        Task<IEnumerable<UserMilestoneClaim>> GetClaimsByUserIdAsync(long userId);
        Task<bool> HasUserClaimedMilestoneAsync(long userId, int milestoneId);
        Task<IEnumerable<UserMilestoneClaim>> GetRecentClaimsAsync(int limit = 10);
        Task<IEnumerable<MilestoneClaimHistoryResponse>> GetClaimHistoryByUserIdAsync(long userId);
    }
}
