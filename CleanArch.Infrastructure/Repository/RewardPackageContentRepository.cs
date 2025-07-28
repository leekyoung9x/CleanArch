using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class RewardPackageContentRepository : BaseRepository<RewardPackageContent>, IRewardPackageContentRepository
    {
        public RewardPackageContentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<RewardPackageContent>> GetContentsByPackageIdAsync(int packageId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM reward_package_contents 
                        WHERE package_id = @PackageId
                        ORDER BY item_id";

                    var result = await connection.QueryAsync<RewardPackageContent>(query, new { PackageId = packageId });
                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<RewardPackageContent>();
                }
            }
        }

        public async Task<bool> DeleteByPackageIdAsync(int packageId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        DELETE FROM reward_package_contents 
                        WHERE package_id = @PackageId";

                    var rowsAffected = await connection.ExecuteAsync(query, new { PackageId = packageId });
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return false;
                }
            }
        }

        // Override base methods to handle composite key
        public override async Task<bool> DeleteAsync(long id)
        {
            throw new NotSupportedException("Use DeleteByPackageIdAsync or provide both PackageId and ItemId");
        }

        public async Task<bool> DeleteAsync(int packageId, int itemId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        DELETE FROM reward_package_contents 
                        WHERE package_id = @PackageId AND item_id = @ItemId";

                    var rowsAffected = await connection.ExecuteAsync(query, new { PackageId = packageId, ItemId = itemId });
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return false;
                }
            }
        }

        public override async Task<RewardPackageContent?> GetByIdAsync(long id)
        {
            throw new NotSupportedException("Use GetContentsByPackageIdAsync or provide both PackageId and ItemId");
        }

        public async Task<RewardPackageContent?> GetByIdAsync(int packageId, int itemId)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM reward_package_contents 
                        WHERE package_id = @PackageId AND item_id = @ItemId";

                    var result = await connection.QuerySingleOrDefaultAsync<RewardPackageContent>(query, new { PackageId = packageId, ItemId = itemId });
                    return result;
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
