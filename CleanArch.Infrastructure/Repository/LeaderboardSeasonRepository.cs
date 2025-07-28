using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class LeaderboardSeasonRepository : BaseRepository<LeaderboardSeason>, ILeaderboardSeasonRepository
    {
        public LeaderboardSeasonRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<LeaderboardSeason> GetActiveSeasonAsync()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM leaderboard_seasons 
                        WHERE status = 'active' 
                        ORDER BY start_time DESC 
                        LIMIT 1";

                    var result = await connection.QuerySingleOrDefaultAsync<LeaderboardSeason>(query);
                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return null;
                }
            }
        }

        public async Task<IEnumerable<LeaderboardSeason>> GetSeasonsByStatusAsync(string status)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM leaderboard_seasons 
                        WHERE status = @Status 
                        ORDER BY start_time DESC";

                    var result = await connection.QueryAsync<LeaderboardSeason>(query, new { Status = status });
                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<LeaderboardSeason>();
                }
            }
        }

        public async Task<LeaderboardSeason> GetSeasonWithRewardsAsync(int seasonId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT ls.*, lr.id as RewardId, lr.season_id as SeasonId, 
                               lr.rank_start as RankStart, lr.rank_end as RankEnd, 
                               lr.reward_package_id as RewardPackageId
                        FROM leaderboard_seasons ls
                        LEFT JOIN leaderboard_rewards lr ON ls.id = lr.season_id
                        WHERE ls.id = @SeasonId
                        ORDER BY lr.rank_start";

                    var seasonDict = new Dictionary<int, LeaderboardSeason>();

                    var result = await connection.QueryAsync<LeaderboardSeason, LeaderboardReward, LeaderboardSeason>(
                        query,
                        (season, reward) =>
                        {
                            if (!seasonDict.TryGetValue(season.Id, out var seasonEntry))
                            {
                                seasonEntry = season;
                                seasonEntry.LeaderboardRewards = new List<LeaderboardReward>();
                                seasonDict.Add(season.Id, seasonEntry);
                            }

                            if (reward != null && reward.Id > 0)
                            {
                                seasonEntry.LeaderboardRewards.Add(reward);
                            }

                            return seasonEntry;
                        },
                        new { SeasonId = seasonId },
                        splitOn: "RewardId"
                    );

                    return seasonDict.Values.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    // Log exception
                    return null;
                }
            }
        }
    }
}
