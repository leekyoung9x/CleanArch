using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class RewardPackageRepository : BaseRepository<RewardPackage>, IRewardPackageRepository
    {
        public RewardPackageRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<RewardPackage> GetByIdWithContentsAsync(int id)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT rp.id, rp.name, rp.description,
                               rpc.package_id as PackageId, rpc.item_id, 
                               rpc.quantity as Quantity, rpc.options
                        FROM reward_packages rp
                        LEFT JOIN reward_package_contents rpc ON rp.id = rpc.package_id
                        WHERE rp.id = @Id";

                    var packageDict = new Dictionary<int, RewardPackage>();

                    var result = await connection.QueryAsync<RewardPackage, RewardPackageContent, RewardPackage>(
                        query,
                        (package, content) =>
                        {
                            if (!packageDict.TryGetValue(package.Id, out var packageEntry))
                            {
                                packageEntry = package;
                                packageEntry.RewardPackageContents = new List<RewardPackageContent>();
                                packageDict.Add(package.Id, packageEntry);
                            }

                            // Chỉ add content khi có ItemId (không null)
                            if (content != null && content.ItemId > 0)
                            {
                                packageEntry.RewardPackageContents.Add(content);
                            }

                            return packageEntry;
                        },
                        new { Id = id },
                        splitOn: "PackageId"
                    );

                    return packageDict.Values.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    // Log exception
                    return null;
                }
            }
        }

        public async Task<IEnumerable<RewardPackage>> GetAllWithContentsAsync()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT rp.id, rp.name, rp.description,
                               rpc.package_id as PackageId, rpc.item_id as ItemId, 
                               rpc.quantity as Quantity, rpc.options as Options
                        FROM reward_packages rp
                        LEFT JOIN reward_package_contents rpc ON rp.id = rpc.package_id
                        ORDER BY rp.id";

                    var packageDict = new Dictionary<int, RewardPackage>();

                    await connection.QueryAsync<RewardPackage, RewardPackageContent, RewardPackage>(
                        query,
                        (package, content) =>
                        {
                            if (!packageDict.TryGetValue(package.Id, out var packageEntry))
                            {
                                packageEntry = package;
                                packageEntry.RewardPackageContents = new List<RewardPackageContent>();
                                packageDict.Add(package.Id, packageEntry);
                            }

                            // Chỉ add content khi có ItemId (không null)
                            if (content != null && content.ItemId > 0)
                            {
                                packageEntry.RewardPackageContents.Add(content);
                            }

                            return packageEntry;
                        },
                        splitOn: "PackageId"
                    );

                    return packageDict.Values;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<RewardPackage>();
                }
            }
        }
    }
}
