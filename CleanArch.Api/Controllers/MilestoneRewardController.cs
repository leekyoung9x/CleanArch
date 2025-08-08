using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MilestoneRewardController : BaseApiController
    {
        public MilestoneRewardController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private int GetAccountId()
        {
            // Lấy ClaimsPrincipal từ HttpContext
            var user = HttpContext.User;

            // Lấy claim từ ClaimsPrincipal
            var idClaim = user.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                throw new UnauthorizedAccessException("Account ID not found in token");

            int claimValue = int.Parse(idClaim);
            return claimValue;
        }

        private int? TryGetAccountId()
        {
            try
            {
                // Lấy ClaimsPrincipal từ HttpContext
                var user = HttpContext.User;

                // Lấy claim từ ClaimsPrincipal
                var idClaim = user?.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(idClaim))
                    return null;

                return int.Parse(idClaim);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<MilestoneReward>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<MilestoneReward>>();

            try
            {
                var data = await _unitOfWork.MilestoneRewards.GetAllAsync();
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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ApiResponse<MilestoneReward>> GetById(int id)
        {
            var apiResponse = new ApiResponse<MilestoneReward>();

            try
            {
                var data = await _unitOfWork.MilestoneRewards.GetByIdAsync(id);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Milestone reward not found";
                }
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

        [HttpGet("with-packages")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<MilestoneReward>>> GetAllWithPackages()
        {
            var apiResponse = new ApiResponse<List<MilestoneReward>>();

            try
            {
                var data = await _unitOfWork.MilestoneRewards.GetAllWithPackagesAsync();
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

        [HttpGet("by-score/{score}")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<MilestoneReward>>> GetByScore(long score)
        {
            var apiResponse = new ApiResponse<List<MilestoneReward>>();

            try
            {
                var data = await _unitOfWork.MilestoneRewards.GetMilestonesByScoreAsync(score);
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

        [HttpGet("required-score/{requiredScore}")]
        [AllowAnonymous]
        public async Task<ApiResponse<MilestoneReward>> GetByRequiredScore(long requiredScore)
        {
            var apiResponse = new ApiResponse<MilestoneReward>();

            try
            {
                var data = await _unitOfWork.MilestoneRewards.GetByRequiredScoreAsync(requiredScore);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Milestone reward not found";
                }
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

        [HttpGet("client-format")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<MilestoneRewardResponse>>> GetMilestoneRewardsForClient()
        {
            var apiResponse = new ApiResponse<List<MilestoneRewardResponse>>();

            try
            {
                int id = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);

                if (playerId == 0)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Bạn chưa tạo nhân vật";
                    return apiResponse;
                }

                // Lấy điểm nạp từ server thay vì truyền từ client
                int userScore = await _unitOfWork.Accounts.GetPlayerAccumulateByPlayerId(playerId);

                var data = await _unitOfWork.MilestoneRewards.GetMilestoneRewardsForClientAsync(playerId, userScore);
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

        [HttpGet("all-milestones")]
        [AllowAnonymous]
        public async Task<ApiResponse<object>> GetAllMilestoneRewardsForClient()
        {
            var apiResponse = new ApiResponse<object>();

            try
            {
                // Thử lấy account ID từ token (có thể null nếu chưa đăng nhập)
                int? accountId = TryGetAccountId();
                int playerId = 0;
                int totalAccumulate = 0;

                // Nếu đã đăng nhập, lấy thông tin player
                if (accountId.HasValue)
                {
                    playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(accountId.Value);

                    if (playerId > 0)
                    {
                        // Lấy tổng điểm nạp từ server
                        totalAccumulate = await _unitOfWork.Accounts.GetPlayerAccumulateByPlayerId(playerId);
                    }
                }

                // Lấy danh sách milestone rewards
                List<MilestoneRewardResponse> milestoneRewards;

                if (playerId > 0)
                {
                    // Đã đăng nhập và có player - lấy đầy đủ thông tin bao gồm claimable/claimed
                    milestoneRewards = (await _unitOfWork.MilestoneRewards.GetAllMilestoneRewardsForClientAsync(playerId, totalAccumulate)).ToList();
                }
                else
                {
                    // Chưa đăng nhập - vẫn lấy đầy đủ milestone rewards nhưng không có thông tin claim
                    // Sử dụng playerId = 0 và userScore = 0 để lấy basic milestone data với items
                    milestoneRewards = (await _unitOfWork.MilestoneRewards.GetAllMilestoneRewardsForClientAsync(0, 0)).ToList();

                    // Set tất cả claimable và claimed = false cho user chưa đăng nhập
                    foreach (var milestone in milestoneRewards)
                    {
                        milestone.Claimable = false;
                        milestone.Claimed = false;
                        milestone.Current = false;
                    }
                }

                apiResponse.Success = true;
                apiResponse.Message = $"Retrieved {milestoneRewards.Count} milestone rewards";
                apiResponse.Result = new
                {
                    TotalAccumulate = totalAccumulate,
                    IsLoggedIn = accountId.HasValue && playerId > 0,
                    MilestoneRewards = milestoneRewards
                };
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
        public async Task<ApiResponse<bool>> Create([FromBody] MilestoneReward milestoneReward)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.MilestoneRewards.AddAsync(milestoneReward);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Milestone reward created successfully" : "Failed to create milestone reward";
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
        public async Task<ApiResponse<bool>> Update([FromBody] MilestoneReward milestoneReward)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.MilestoneRewards.UpdateAsync(milestoneReward);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Milestone reward updated successfully" : "Failed to update milestone reward";
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

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> Delete(int id)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.MilestoneRewards.DeleteAsync(id);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Milestone reward deleted successfully" : "Failed to delete milestone reward";
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
    }
}
