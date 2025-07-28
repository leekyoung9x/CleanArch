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
    public class UserMilestoneClaimController : BaseApiController
    {
        public UserMilestoneClaimController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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
    }
}
