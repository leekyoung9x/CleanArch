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
    public class LeaderboardRewardController : BaseApiController
    {
        public LeaderboardRewardController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<LeaderboardReward>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<LeaderboardReward>>();

            try
            {
                var data = await _unitOfWork.LeaderboardRewards.GetAllAsync();
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
        public async Task<ApiResponse<LeaderboardReward>> GetById(int id)
        {
            var apiResponse = new ApiResponse<LeaderboardReward>();

            try
            {
                var data = await _unitOfWork.LeaderboardRewards.GetByIdAsync(id);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Leaderboard reward not found";
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

        [HttpGet("season/{seasonId}")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<LeaderboardReward>>> GetBySeasonId(int seasonId)
        {
            var apiResponse = new ApiResponse<List<LeaderboardReward>>();

            try
            {
                var data = await _unitOfWork.LeaderboardRewards.GetRewardsBySeasonIdAsync(seasonId);
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

        [HttpGet("season/{seasonId}/rank/{rank}")]
        [AllowAnonymous]
        public async Task<ApiResponse<LeaderboardReward>> GetByRank(int seasonId, int rank)
        {
            var apiResponse = new ApiResponse<LeaderboardReward>();

            try
            {
                var data = await _unitOfWork.LeaderboardRewards.GetRewardByRankAsync(seasonId, rank);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Leaderboard reward not found for this rank";
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
        public async Task<ApiResponse<List<LeaderboardReward>>> GetAllWithPackages()
        {
            var apiResponse = new ApiResponse<List<LeaderboardReward>>();

            try
            {
                var data = await _unitOfWork.LeaderboardRewards.GetAllWithPackagesAsync();
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
        public async Task<ApiResponse<bool>> Create([FromBody] LeaderboardReward leaderboardReward)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.LeaderboardRewards.AddAsync(leaderboardReward);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Leaderboard reward created successfully" : "Failed to create leaderboard reward";
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
        public async Task<ApiResponse<bool>> Update([FromBody] LeaderboardReward leaderboardReward)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.LeaderboardRewards.UpdateAsync(leaderboardReward);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Leaderboard reward updated successfully" : "Failed to update leaderboard reward";
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
                var result = await _unitOfWork.LeaderboardRewards.DeleteAsync(id);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Leaderboard reward deleted successfully" : "Failed to delete leaderboard reward";
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
