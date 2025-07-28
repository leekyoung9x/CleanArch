using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class UserMilestoneClaimRepository : BaseRepository<UserMilestoneClaim>, IUserMilestoneClaimRepository
    {
        public UserMilestoneClaimRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<UserMilestoneClaim>> GetClaimsByUserIdAsync(long userId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT umc.*, mr.milestone_name as MilestoneName, 
                               mr.required_score as RequiredScore
                        FROM user_milestone_claims umc
                        LEFT JOIN milestone_rewards mr ON umc.milestone_id = mr.id
                        WHERE umc.user_id = @UserId
                        ORDER BY umc.claimed_at DESC";

                    var result = await connection.QueryAsync<UserMilestoneClaim, MilestoneReward, UserMilestoneClaim>(
                        query,
                        (claim, milestone) =>
                        {
                            if (milestone != null)
                            {
                                claim.MilestoneReward = milestone;
                            }
                            return claim;
                        },
                        new { UserId = userId },
                        splitOn: "MilestoneName"
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<UserMilestoneClaim>();
                }
            }
        }

        public async Task<bool> HasUserClaimedMilestoneAsync(long userId, int milestoneId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT COUNT(1) FROM user_milestone_claims 
                        WHERE user_id = @UserId AND milestone_id = @MilestoneId";

                    var count = await connection.QuerySingleAsync<int>(query, new { UserId = userId, MilestoneId = milestoneId });
                    return count > 0;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return false;
                }
            }
        }

        public async Task<IEnumerable<UserMilestoneClaim>> GetRecentClaimsAsync(int limit = 10)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT umc.*, mr.milestone_name as MilestoneName, 
                               mr.required_score as RequiredScore
                        FROM user_milestone_claims umc
                        LEFT JOIN milestone_rewards mr ON umc.milestone_id = mr.id
                        ORDER BY umc.claimed_at DESC
                        LIMIT @Limit";

                    var result = await connection.QueryAsync<UserMilestoneClaim, MilestoneReward, UserMilestoneClaim>(
                        query,
                        (claim, milestone) =>
                        {
                            if (milestone != null)
                            {
                                claim.MilestoneReward = milestone;
                            }
                            return claim;
                        },
                        new { Limit = limit },
                        splitOn: "MilestoneName"
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<UserMilestoneClaim>();
                }
            }
        }
    }
}
