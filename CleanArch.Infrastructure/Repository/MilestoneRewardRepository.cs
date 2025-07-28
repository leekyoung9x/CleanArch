using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;
using CleanArch.Core.Utils;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class MilestoneRewardRepository : BaseRepository<MilestoneReward>, IMilestoneRewardRepository
    {
        public MilestoneRewardRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<MilestoneReward>> GetMilestonesByScoreAsync(long userScore)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM milestone_rewards 
                        WHERE required_score <= @UserScore
                        ORDER BY required_score DESC";

                    var result = await connection.QueryAsync<MilestoneReward>(query, new { UserScore = userScore });
                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<MilestoneReward>();
                }
            }
        }

        public async Task<MilestoneReward> GetByRequiredScoreAsync(long requiredScore)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT * FROM milestone_rewards 
                        WHERE required_score = @RequiredScore";

                    var result = await connection.QuerySingleOrDefaultAsync<MilestoneReward>(query, new { RequiredScore = requiredScore });
                    return result;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return null;
                }
            }
        }

        public async Task<IEnumerable<MilestoneReward>> GetAllWithPackagesAsync()
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT mr.*, rp.id as PackageId, rp.name as PackageName, 
                               rp.description as PackageDescription
                        FROM milestone_rewards mr
                        LEFT JOIN reward_packages rp ON mr.reward_package_id = rp.id
                        ORDER BY mr.required_score";

                    var milestoneDict = new Dictionary<int, MilestoneReward>();

                    var result = await connection.QueryAsync<MilestoneReward, RewardPackage, MilestoneReward>(
                        query,
                        (milestone, package) =>
                        {
                            if (!milestoneDict.TryGetValue(milestone.Id, out var milestoneEntry))
                            {
                                milestoneEntry = milestone;
                                milestoneDict.Add(milestone.Id, milestoneEntry);
                            }

                            if (package != null && package.Id > 0)
                            {
                                milestoneEntry.RewardPackage = package;
                            }

                            return milestoneEntry;
                        },
                        splitOn: "PackageId"
                    );

                    return milestoneDict.Values;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<MilestoneReward>();
                }
            }
        }

        public async Task<IEnumerable<MilestoneRewardResponse>> GetMilestoneRewardsForClientAsync(long userId, long userScore)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT 
                            mr.id,
                            mr.required_score as Amount,
                            CASE WHEN umc.user_id IS NOT NULL THEN 1 ELSE 0 END as Claimed,
                            CASE WHEN mr.required_score <= @UserScore AND umc.user_id IS NULL THEN 1 ELSE 0 END as Claimable,
                            CASE WHEN mr.required_score = (
                                SELECT MIN(required_score) 
                                FROM milestone_rewards 
                                WHERE required_score > @UserScore
                            ) THEN 1 ELSE 0 END as Current,
                            rpc.item_id,
                            rpc.quantity,
                            rpc.options,
                            it.NAME as ItemName,
                            it.icon_id as ItemIconId,
                            it.TYPE as ItemType,
                            it.description as ItemDescription
                        FROM milestone_rewards mr
                        LEFT JOIN user_milestone_claims umc ON mr.id = umc.milestone_id AND umc.user_id = @UserId
                        LEFT JOIN reward_package_contents rpc ON mr.reward_package_id = rpc.package_id
                        LEFT JOIN item_template it ON rpc.item_id = it.id
                        ORDER BY mr.required_score, rpc.item_id";

                    var result = await connection.QueryAsync(query, new { UserId = userId, UserScore = userScore });

                    // Group và process dữ liệu
                    var milestoneDict = new Dictionary<int, MilestoneRewardResponse>();

                    foreach (var row in result)
                    {
                        var milestoneId = (int)row.id;
                        
                        if (!milestoneDict.TryGetValue(milestoneId, out var milestone))
                        {
                            milestone = new MilestoneRewardResponse
                            {
                                Id = milestoneId,
                                Amount = row.Amount,
                                Claimed = row.Claimed == 1,
                                Claimable = row.Claimable == 1,
                                Current = row.Current == 1,
                                Items = new List<MilestoneItemResponse>()
                            };
                            milestoneDict.Add(milestoneId, milestone);
                        }

                        if (row.item_id != null)
                        {
                            var itemId = row.item_id.ToString();
                            var existingItem = milestone.Items.FirstOrDefault(i => i.Id == itemId);
                            
                            if (existingItem == null)
                            {
                                var itemResponse = new MilestoneItemResponse
                                {
                                    Id = itemId,
                                    Icon = IconHelper.GetIconByIconId(row.ItemIconId ?? 0), // Sử dụng IconHelper
                                    Qty = row.quantity ?? 1,
                                    Name = row.ItemName ?? "Unknown Item",
                                    Stats = new List<ItemStatResponse>()
                                };

                                // Add basic stats based on ItemType
                                var itemTypeText = GetItemTypeText(row.ItemType);
                                if (!string.IsNullOrEmpty(itemTypeText))
                                {
                                    itemResponse.Stats.Add(new ItemStatResponse { Label = "Loại", Value = itemTypeText });
                                }
                                
                                if (!string.IsNullOrEmpty(row.ItemDescription))
                                {
                                    itemResponse.Stats.Add(new ItemStatResponse { Label = "Mô tả", Value = row.ItemDescription });
                                }

                                // Process options from JSON - Method 1
                                if (!string.IsNullOrEmpty(row.options))
                                {
                                    try
                                    {
                                        var options = JsonConvert.DeserializeObject<List<RewardPackageContentOption>>(row.options);
                                        foreach (var option in options)
                                        {
                                            // Query để lấy thông tin option từ item_option_template
                                            const string optionQuery = @"
                                                SELECT id, NAME, TYPE 
                                                FROM item_option_template 
                                                WHERE id = @OptionId";
                                            
                                            var optionInfo = await connection.QuerySingleOrDefaultAsync(
                                                optionQuery, 
                                                new { OptionId = option.Id }
                                            );

                                            if (optionInfo != null)
                                            {
                                                var optionName = optionInfo.NAME ?? "Unknown";
                                                var optionValue = option.Value?.ToString() ?? "0";
                                                
                                                // Replace # with actual value in option name
                                                if (optionName.Contains("#"))
                                                {
                                                    optionName = optionName.Replace("#", optionValue);
                                                }
                                                
                                                var optionTypeText = GetOptionTypeText(optionInfo.TYPE);
                                                itemResponse.Stats.Add(new ItemStatResponse 
                                                { 
                                                    Label = optionTypeText ?? "Thuộc tính", 
                                                    Value = optionName 
                                                });
                                            }
                                        }
                                    }
                                    catch (JsonException)
                                    {
                                        // If JSON parsing fails, ignore options
                                    }
                                }

                                milestone.Items.Add(itemResponse);
                            }
                        }
                    }

                    return milestoneDict.Values.OrderBy(m => m.Amount);
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<MilestoneRewardResponse>();
                }
            }
        }

        private string GetItemTypeText(int? itemType)
        {
            if (!itemType.HasValue) return "Unknown";
            
            return itemType.Value switch
            {
                0 => "Áo",
                1 => "Quần",
                2 => "Găng tay",
                3 => "Giày",
                4 => "Rada",
                5 => "Áo choàng",
                6 => "Nhẫn",
                7 => "Chuỗi",
                8 => "Vũ khí",
                9 => "Vật phẩm tiêu hao",
                10 => "Vàng",
                11 => "Hộp quà",
                12 => "Ngọc",
                _ => $"Type {itemType.Value}"
            };
        }

        private string GetOptionTypeText(int? optionType)
        {
            if (!optionType.HasValue) return "Thuộc tính";
            
            return optionType.Value switch
            {
                0 => "Sức mạnh",
                1 => "Thể lực", 
                2 => "Chính xác",
                3 => "Phản ứng",
                4 => "HP",
                5 => "MP",
                6 => "Tấn công",
                7 => "Phòng thủ",
                8 => "Tỷ lệ chí mạng",
                9 => "Sát thương chí mạng",
                10 => "Tránh né",
                _ => $"Thuộc tính {optionType.Value}"
            };
        }

        public async Task<IEnumerable<MilestoneRewardResponse>> GetAllMilestoneRewardsForClientAsync(long userId, long userScore)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = @"
                        SELECT 
                            mr.id,
                            mr.milestone_name,
                            mr.required_score as Amount,
                            CASE WHEN umc.user_id IS NOT NULL THEN 1 ELSE 0 END as Claimed,
                            CASE WHEN mr.required_score <= @UserScore AND umc.user_id IS NULL THEN 1 ELSE 0 END as Claimable,
                            CASE WHEN mr.required_score = (
                                SELECT MIN(required_score) 
                                FROM milestone_rewards 
                                WHERE required_score > @UserScore
                            ) THEN 1 ELSE 0 END as Current,
                            rpc.item_id,
                            rpc.quantity,
                            rpc.options,
                            it.NAME as ItemName,
                            it.icon_id as ItemIconId,
                            it.TYPE as ItemType,
                            it.description as ItemDescription
                        FROM milestone_rewards mr
                        LEFT JOIN user_milestone_claims umc ON mr.id = umc.milestone_id AND umc.user_id = @UserId
                        LEFT JOIN reward_package_contents rpc ON mr.reward_package_id = rpc.package_id
                        LEFT JOIN item_template it ON rpc.item_id = it.id
                        ORDER BY mr.required_score ASC, rpc.item_id";

                    var result = await connection.QueryAsync(query, new { UserId = userId, UserScore = userScore });

                    // Group và process dữ liệu
                    var milestoneDict = new Dictionary<int, MilestoneRewardResponse>();

                    foreach (var row in result)
                    {
                        var milestoneId = (int)row.id;
                        
                        if (!milestoneDict.TryGetValue(milestoneId, out var milestone))
                        {
                            milestone = new MilestoneRewardResponse
                            {
                                Id = milestoneId,
                                Amount = row.Amount,
                                Claimed = row.Claimed == 1,
                                Claimable = row.Claimable == 1,
                                Current = row.Current == 1,
                                Items = new List<MilestoneItemResponse>()
                            };
                            milestoneDict.Add(milestoneId, milestone);
                        }

                        if (row.item_id != null)
                        {
                            var itemId = row.item_id.ToString();
                            var existingItem = milestone.Items.FirstOrDefault(i => i.Id == itemId);
                            
                            if (existingItem == null)
                            {
                                var itemResponse = new MilestoneItemResponse
                                {
                                    Id = itemId,
                                    Icon = IconHelper.GetIconByIconId(row.ItemIconId ?? 0),
                                    Qty = row.quantity ?? 1,
                                    Name = row.ItemName ?? "Unknown Item",
                                    Stats = new List<ItemStatResponse>()
                                };

                                // Add basic stats based on ItemType
                                var itemTypeText = GetItemTypeText(row.ItemType);
                                if (!string.IsNullOrEmpty(itemTypeText))
                                {
                                    itemResponse.Stats.Add(new ItemStatResponse { Label = "Loại", Value = itemTypeText });
                                }
                                
                                if (!string.IsNullOrEmpty(row.ItemDescription))
                                {
                                    itemResponse.Stats.Add(new ItemStatResponse { Label = "Mô tả", Value = row.ItemDescription });
                                }

                                // Process options from JSON - Method 2  
                                if (!string.IsNullOrEmpty(row.options))
                                {
                                    try
                                    {
                                        var options = JsonConvert.DeserializeObject<List<RewardPackageContentOption>>(row.options);
                                        foreach (var option in options)
                                        {
                                            // Query để lấy thông tin option từ item_option_template
                                            const string optionQuery = @"
                                                SELECT id, NAME, TYPE 
                                                FROM item_option_template 
                                                WHERE id = @OptionId";
                                            
                                            var optionInfo = await connection.QuerySingleOrDefaultAsync(
                                                optionQuery, 
                                                new { OptionId = option.Id }
                                            );

                                            if (optionInfo != null)
                                            {
                                                var optionName = optionInfo.NAME ?? "Unknown";
                                                var optionValue = option.Value?.ToString() ?? "0";
                                                
                                                // Replace # with actual value in option name
                                                if (optionName.Contains("#"))
                                                {
                                                    optionName = optionName.Replace("#", optionValue);
                                                }
                                                
                                                var optionTypeText = GetOptionTypeText(optionInfo.TYPE);
                                                itemResponse.Stats.Add(new ItemStatResponse 
                                                { 
                                                    Label = optionTypeText ?? "Thuộc tính", 
                                                    Value = optionName 
                                                });
                                            }
                                        }
                                    }
                                    catch (JsonException)
                                    {
                                        // If JSON parsing fails, ignore options
                                    }
                                }

                                milestone.Items.Add(itemResponse);
                            }
                        }
                    }

                    return milestoneDict.Values.OrderBy(m => m.Amount);
                }
                catch (Exception ex)
                {
                    // Log exception
                    return new List<MilestoneRewardResponse>();
                }
            }
        }
    }
}
