using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class UserLeaderboardClaimRepository : BaseRepository<UserLeaderboardClaim>, IUserLeaderboardClaimRepository
    {
        public UserLeaderboardClaimRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<UserLeaderboardClaim>> GetClaimsByUserIdAsync(long userId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT ulc.*, ls.id as SeasonId, ls.season_name as SeasonName, 
                               ls.start_time as StartTime, ls.end_time as EndTime, 
                               ls.status as Status
                        FROM user_leaderboard_claims ulc
                        LEFT JOIN leaderboard_seasons ls ON ulc.season_id = ls.id
                        WHERE ulc.user_id = @UserId
                        ORDER BY ulc.claimed_at DESC";

                    var result = await connection.QueryAsync<UserLeaderboardClaim, LeaderboardSeason, UserLeaderboardClaim>(
                        query,
                        (claim, season) =>
                        {
                            if (season != null)
                            {
                                claim.LeaderboardSeason = season;
                            }
                            return claim;
                        },
                        new { UserId = userId },
                        splitOn: "SeasonId"
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<UserLeaderboardClaim>();
                }
            }
        }

        public async Task<UserLeaderboardClaim> GetClaimByUserAndSeasonAsync(long userId, int seasonId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT ulc.*, ls.id as SeasonId, ls.season_name as SeasonName, 
                               ls.start_time as StartTime, ls.end_time as EndTime, 
                               ls.status as Status
                        FROM user_leaderboard_claims ulc
                        LEFT JOIN leaderboard_seasons ls ON ulc.season_id = ls.id
                        WHERE ulc.user_id = @UserId AND ulc.season_id = @SeasonId";

                    var result = await connection.QueryAsync<UserLeaderboardClaim, LeaderboardSeason, UserLeaderboardClaim>(
                        query,
                        (claim, season) =>
                        {
                            if (season != null)
                            {
                                claim.LeaderboardSeason = season;
                            }
                            return claim;
                        },
                        new { UserId = userId, SeasonId = seasonId },
                        splitOn: "SeasonId"
                    );

                    return result.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    // Log exception
                    return null;
                }
            }
        }

        public async Task<bool> HasUserClaimedSeasonAsync(long userId, int seasonId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT COUNT(1) FROM user_leaderboard_claims 
                        WHERE user_id = @UserId AND season_id = @SeasonId";

                    var count = await connection.QuerySingleAsync<int>(query, new { UserId = userId, SeasonId = seasonId });
                    return count > 0;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return false;
                }
            }
        }

        public async Task<IEnumerable<UserLeaderboardClaim>> GetClaimsBySeasonIdAsync(int seasonId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT ulc.*, ls.id as SeasonId, ls.season_name as SeasonName, 
                               ls.start_time as StartTime, ls.end_time as EndTime, 
                               ls.status as Status
                        FROM user_leaderboard_claims ulc
                        LEFT JOIN leaderboard_seasons ls ON ulc.season_id = ls.id
                        WHERE ulc.season_id = @SeasonId
                        ORDER BY ulc.final_rank";

                    var result = await connection.QueryAsync<UserLeaderboardClaim, LeaderboardSeason, UserLeaderboardClaim>(
                        query,
                        (claim, season) =>
                        {
                            if (season != null)
                            {
                                claim.LeaderboardSeason = season;
                            }
                            return claim;
                        },
                        new { SeasonId = seasonId },
                        splitOn: "SeasonId"
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<UserLeaderboardClaim>();
                }
            }
        }
    }
}
