using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLeaderboardClaimController : BaseApiController
    {
        public UserLeaderboardClaimController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<UserLeaderboardClaim>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<UserLeaderboardClaim>>();

            try
            {
                var data = await _unitOfWork.UserLeaderboardClaims.GetAllAsync();
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
        public async Task<ApiResponse<List<UserLeaderboardClaim>>> GetByUserId(long userId)
        {
            var apiResponse = new ApiResponse<List<UserLeaderboardClaim>>();

            try
            {
                var data = await _unitOfWork.UserLeaderboardClaims.GetClaimsByUserIdAsync(userId);
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

        [HttpGet("user/{userId}/season/{seasonId}")]
        [AllowAnonymous]
        public async Task<ApiResponse<UserLeaderboardClaim>> GetByUserAndSeason(long userId, int seasonId)
        {
            var apiResponse = new ApiResponse<UserLeaderboardClaim>();

            try
            {
                var data = await _unitOfWork.UserLeaderboardClaims.GetClaimByUserAndSeasonAsync(userId, seasonId);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Claim not found";
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

        [HttpGet("user/{userId}/season/{seasonId}/claimed")]
        [AllowAnonymous]
        public async Task<ApiResponse<bool>> HasUserClaimedSeason(long userId, int seasonId)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.UserLeaderboardClaims.HasUserClaimedSeasonAsync(userId, seasonId);
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

        [HttpGet("season/{seasonId}")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<UserLeaderboardClaim>>> GetBySeasonId(int seasonId)
        {
            var apiResponse = new ApiResponse<List<UserLeaderboardClaim>>();

            try
            {
                var data = await _unitOfWork.UserLeaderboardClaims.GetClaimsBySeasonIdAsync(seasonId);
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
        public async Task<ApiResponse<bool>> Create([FromBody] UserLeaderboardClaim claim)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                // Check if user has already claimed this season
                var alreadyClaimed = await _unitOfWork.UserLeaderboardClaims.HasUserClaimedSeasonAsync(claim.UserId, claim.SeasonId);
                if (alreadyClaimed)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "User has already claimed reward for this season";
                    return apiResponse;
                }

                var result = await _unitOfWork.UserLeaderboardClaims.AddAsync(claim);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Leaderboard claim created successfully" : "Failed to create leaderboard claim";
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
        public async Task<ApiResponse<bool>> Update([FromBody] UserLeaderboardClaim claim)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.UserLeaderboardClaims.UpdateAsync(claim);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Leaderboard claim updated successfully" : "Failed to update leaderboard claim";
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
