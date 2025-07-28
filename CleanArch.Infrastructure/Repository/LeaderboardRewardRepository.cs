using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class LeaderboardRewardRepository : BaseRepository<LeaderboardReward>, ILeaderboardRewardRepository
    {
        public LeaderboardRewardRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<LeaderboardReward>> GetRewardsBySeasonIdAsync(int seasonId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT lr.*, rp.id as PackageId, rp.name as PackageName, 
                               rp.description as PackageDescription
                        FROM leaderboard_rewards lr
                        LEFT JOIN reward_packages rp ON lr.reward_package_id = rp.id
                        WHERE lr.season_id = @SeasonId
                        ORDER BY lr.rank_start";

                    var result = await connection.QueryAsync<LeaderboardReward, RewardPackage, LeaderboardReward>(
                        query,
                        (reward, package) =>
                        {
                            if (package != null)
                            {
                                reward.RewardPackage = package;
                            }
                            return reward;
                        },
                        new { SeasonId = seasonId },
                        splitOn: "PackageId"
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<LeaderboardReward>();
                }
            }
        }

        public async Task<LeaderboardReward> GetRewardByRankAsync(int seasonId, int rank)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT lr.*, rp.id as PackageId, rp.name as PackageName, 
                               rp.description as PackageDescription
                        FROM leaderboard_rewards lr
                        LEFT JOIN reward_packages rp ON lr.reward_package_id = rp.id
                        WHERE lr.season_id = @SeasonId 
                        AND @Rank BETWEEN lr.rank_start AND lr.rank_end
                        LIMIT 1";

                    var result = await connection.QueryAsync<LeaderboardReward, RewardPackage, LeaderboardReward>(
                        query,
                        (reward, package) =>
                        {
                            if (package != null)
                            {
                                reward.RewardPackage = package;
                            }
                            return reward;
                        },
                        new { SeasonId = seasonId, Rank = rank },
                        splitOn: "PackageId"
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

        public async Task<IEnumerable<LeaderboardReward>> GetAllWithPackagesAsync()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT lr.*, rp.id as PackageId, rp.name as PackageName, 
                               rp.description as PackageDescription,
                               ls.id as SeasonId, ls.season_name as SeasonName, 
                               ls.status as SeasonStatus
                        FROM leaderboard_rewards lr
                        LEFT JOIN reward_packages rp ON lr.reward_package_id = rp.id
                        LEFT JOIN leaderboard_seasons ls ON lr.season_id = ls.id
                        ORDER BY lr.season_id, lr.rank_start";

                    var result = await connection.QueryAsync<LeaderboardReward, RewardPackage, LeaderboardSeason, LeaderboardReward>(
                        query,
                        (reward, package, season) =>
                        {
                            if (package != null)
                            {
                                reward.RewardPackage = package;
                            }
                            if (season != null)
                            {
                                reward.LeaderboardSeason = season;
                            }
                            return reward;
                        },
                        splitOn: "PackageId,SeasonId"
                    );

                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<LeaderboardReward>();
                }
            }
        }
    }
}
