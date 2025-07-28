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
    public class LeaderboardSeasonController : BaseApiController
    {
        public LeaderboardSeasonController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<LeaderboardSeason>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<LeaderboardSeason>>();

            try
            {
                var data = await _unitOfWork.LeaderboardSeasons.GetAllAsync();
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
        public async Task<ApiResponse<LeaderboardSeason>> GetById(int id)
        {
            var apiResponse = new ApiResponse<LeaderboardSeason>();

            try
            {
                var data = await _unitOfWork.LeaderboardSeasons.GetByIdAsync(id);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Season not found";
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

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<ApiResponse<LeaderboardSeason>> GetActiveSeason()
        {
            var apiResponse = new ApiResponse<LeaderboardSeason>();

            try
            {
                var data = await _unitOfWork.LeaderboardSeasons.GetActiveSeasonAsync();
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "No active season found";
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

        [HttpGet("status/{status}")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<LeaderboardSeason>>> GetByStatus(string status)
        {
            var apiResponse = new ApiResponse<List<LeaderboardSeason>>();

            try
            {
                var data = await _unitOfWork.LeaderboardSeasons.GetSeasonsByStatusAsync(status);
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

        [HttpGet("{id}/rewards")]
        [AllowAnonymous]
        public async Task<ApiResponse<LeaderboardSeason>> GetSeasonWithRewards(int id)
        {
            var apiResponse = new ApiResponse<LeaderboardSeason>();

            try
            {
                var data = await _unitOfWork.LeaderboardSeasons.GetSeasonWithRewardsAsync(id);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Season not found";
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

        [HttpPost]
        public async Task<ApiResponse<bool>> Create([FromBody] LeaderboardSeason season)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.LeaderboardSeasons.AddAsync(season);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Season created successfully" : "Failed to create season";
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
        public async Task<ApiResponse<bool>> Update([FromBody] LeaderboardSeason season)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.LeaderboardSeasons.UpdateAsync(season);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Season updated successfully" : "Failed to update season";
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
                var result = await _unitOfWork.LeaderboardSeasons.DeleteAsync(id);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Season deleted successfully" : "Failed to delete season";
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
