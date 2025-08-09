using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using RewardPackageContentOption = CleanArch.Core.Entities.RewardPackageContentOption;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMilestoneClaimController : BaseApiController
    {
        private readonly IConfiguration _configuration;

        public UserMilestoneClaimController(IUnitOfWork unitOfWork, IConfiguration configuration) : base(unitOfWork)
        {
            _configuration = configuration;
        }

        private int GetAccountId()
        {
            // Lấy ClaimsPrincipal từ HttpContext
            var user = HttpContext.User;

            // Lấy claim từ ClaimsPrincipal
            int claimValue = int.Parse(user.FindFirst("id")?.Value);
            return claimValue;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<UserMilestoneClaim>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<UserMilestoneClaim>>();

            try
            {
                var data = await _unitOfWork.UserMilestoneClaims.GetAllAsync();
                apiResponse.Success = true;
                apiResponse.Result = data.ToList();
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<UserMilestoneClaim>>> GetByUserId(long userId)
        {
            var apiResponse = new ApiResponse<List<UserMilestoneClaim>>();

            try
            {
                var data = await _unitOfWork.UserMilestoneClaims.GetClaimsByUserIdAsync(userId);
                apiResponse.Success = true;
                apiResponse.Result = data.ToList();
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }

        [HttpGet("user/{userId}/milestone/{milestoneId}/claimed")]
        [AllowAnonymous]
        public async Task<ApiResponse<bool>> HasUserClaimedMilestone(long userId, int milestoneId)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.UserMilestoneClaims.HasUserClaimedMilestoneAsync(userId, milestoneId);
                apiResponse.Success = true;
                apiResponse.Result = result;
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }

        [HttpGet("recent")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<UserMilestoneClaim>>> GetRecentClaims([FromQuery] int limit = 10)
        {
            var apiResponse = new ApiResponse<List<UserMilestoneClaim>>();

            try
            {
                var data = await _unitOfWork.UserMilestoneClaims.GetRecentClaimsAsync(limit);
                apiResponse.Success = true;
                apiResponse.Result = data.ToList();
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }

        [HttpPost]
        public async Task<ApiResponse<bool>> Create([FromBody] UserMilestoneClaim claim)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                // Check if user has already claimed this milestone
                var alreadyClaimed = await _unitOfWork.UserMilestoneClaims.HasUserClaimedMilestoneAsync(claim.UserId, claim.MilestoneId);
                if (alreadyClaimed)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "User has already claimed this milestone";
                    return apiResponse;
                }

                var result = await _unitOfWork.UserMilestoneClaims.AddAsync(claim);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Milestone claim created successfully" : "Failed to create milestone claim";
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }

        [HttpPut]
        public async Task<ApiResponse<bool>> Update([FromBody] UserMilestoneClaim claim)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.UserMilestoneClaims.UpdateAsync(claim);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Milestone claim updated successfully" : "Failed to update milestone claim";
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }

        [HttpPost("claim/{milestoneId}")]
        public async Task<ServiceResult> ClaimMilestone(int milestoneId)
        {
            var result = new ServiceResult();
            result.Status = false;
            result.StatusMessage = "Có lỗi xảy ra";

            try
            {
                // Lấy thông tin user từ token
                int accountId = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(accountId);

                if (playerId == 0)
                {
                    result.StatusMessage = "Bạn chưa tạo nhân vật";
                    return result;
                }

                // Kiểm tra xem đã nhận thưởng chưa
                bool alreadyClaimed = await _unitOfWork.UserMilestoneClaims.HasUserClaimedMilestoneAsync(playerId, milestoneId);
                if (alreadyClaimed)
                {
                    result.StatusMessage = "Bạn đã nhận thưởng mốc này rồi";
                    return result;
                }

                // Lấy thông tin mốc thưởng
                var milestone = await _unitOfWork.MilestoneRewards.GetByIdAsync(milestoneId);
                if (milestone == null)
                {
                    result.StatusMessage = "Mốc thưởng không tồn tại";
                    return result;
                }

                // Lấy số tiền đã nạp của người chơi
                int userAccumulate = await _unitOfWork.Accounts.GetPlayerAccumulateByPlayerId(playerId);

                // Kiểm tra điều kiện đạt mốc
                if (userAccumulate < milestone.RequiredScore)
                {
                    result.StatusMessage = "Bạn chưa đủ điều kiện nhận mốc này";
                    return result;
                }

                // Lấy thông tin gói quà
                var rewardPackage = await _unitOfWork.RewardPackages.GetByIdWithContentsAsync(milestone.RewardPackageId);
                if (rewardPackage == null)
                {
                    result.StatusMessage = "Gói quà không tồn tại";
                    return result;
                }

                // Chuyển đổi gói quà thành gift code
                var giftCodeResult = await ConvertRewardPackageToGiftCode(rewardPackage);
                if (giftCodeResult == null || string.IsNullOrEmpty(giftCodeResult.Code))
                {
                    result.StatusMessage = "Không thể tạo gift code";
                    return result;
                }

                // Sử dụng transaction để đảm bảo tính nhất quán
                using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Lưu lịch sử nhận thưởng
                            var claim = new UserMilestoneClaim
                            {
                                UserId = playerId,
                                MilestoneId = milestoneId,
                                ClaimedAt = DateTime.Now,
                                GiftCodeId = giftCodeResult.Id
                            };

                            bool success = await _unitOfWork.UserMilestoneClaims.AddAsync(claim, connection, transaction);
                            if (success)
                            {
                                transaction.Commit();
                                result.Status = true;
                                result.StatusMessage = "Nhận thưởng thành công";
                                result.Data = giftCodeResult.Code;
                            }
                            else
                            {
                                transaction.Rollback();
                                result.StatusMessage = "Lưu lịch sử nhận thưởng thất bại";
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            result.StatusMessage = $"Lỗi database: {ex.Message}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.StatusMessage = $"Lỗi: {ex.Message}";
            }

            return result;
        }

        [HttpGet("history")]
        public async Task<ServiceResult> GetMyClaimHistory()
        {
            var result = new ServiceResult();
            result.Status = false;
            result.StatusMessage = "Có lỗi xảy ra";

            try
            {
                // Lấy thông tin user từ token
                int accountId = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(accountId);

                if (playerId == 0)
                {
                    result.StatusMessage = "Bạn chưa tạo nhân vật";
                    return result;
                }

                // Lấy lịch sử nhận thưởng
                var history = await _unitOfWork.UserMilestoneClaims.GetClaimHistoryByUserIdAsync(playerId);

                result.Status = true;
                result.StatusMessage = "Lấy lịch sử thành công";
                result.Data = history.ToList();
            }
            catch (Exception ex)
            {
                result.StatusMessage = $"Lỗi: {ex.Message}";
            }

            return result;
        }

        private async Task<GiftCode> ConvertRewardPackageToGiftCode(RewardPackage rewardPackage)
        {
            try
            {
                // Tạo danh sách items cho gift code
                var giftCodeItems = new List<GiftCodeItem>();

                foreach (var content in rewardPackage.RewardPackageContents)
                {
                    var giftCodeItem = new GiftCodeItem
                    {
                        Id = content.ItemId,
                        Quantity = content.Quantity,
                        Options = new List<GiftCodeOption>()
                    };

                    // Chuyển đổi options từ JSON nếu có
                    if (!string.IsNullOrEmpty(content.Options))
                    {
                        try
                        {
                            var options = JsonConvert.DeserializeObject<List<RewardPackageContentOption>>(content.Options);
                            foreach (var option in options ?? new List<RewardPackageContentOption>())
                            {
                                giftCodeItem.Options.Add(new GiftCodeOption
                                {
                                    Id = option.Id,
                                    Param = option.Value ?? 0
                                });
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore invalid JSON options
                        }
                    }

                    giftCodeItems.Add(giftCodeItem);
                }

                // Tạo gift code entity
                var giftCode = new GiftCode
                {
                    Type = 2, // Personal gift code
                    Code = await _unitOfWork.GiftCodes.GenerateUniqueGiftCodeAsync(),
                    Gold = 0,
                    Gem = 0,
                    Ruby = 0,
                    Items = JsonConvert.SerializeObject(giftCodeItems).ToLower(),
                    Status = 0, // Active
                    Active = 0, // Không yêu cầu kích hoạt
                    ExpiresAt = DateTime.Now.AddYears(5), // Hết hạn sau 5 năm
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Lưu gift code vào database và lấy ID
                long giftCodeId = await _unitOfWork.GiftCodes.AddAndReturnIdAsync(giftCode);
                if (giftCodeId > 0)
                {
                    giftCode.Id = giftCodeId;
                    return giftCode;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
