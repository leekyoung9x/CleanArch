using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RankController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Power")]
        public async Task<ApiResponse<List<rank>>> GetPower()
        {
            var apiResponse = new ApiResponse<List<rank>>();

            try
            {
                var data = await _unitOfWork.Ranks.GetPowerRank();
                apiResponse.Success = true;
                apiResponse.Result = data;
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

        [HttpGet("Vnd")]
        public async Task<ApiResponse<List<rank>>> GetVnd()
        {
            var apiResponse = new ApiResponse<List<rank>>();

            try
            {
                var data = await _unitOfWork.Ranks.GetVndRank();
                apiResponse.Success = true;
                apiResponse.Result = data;
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

        [HttpGet("PetPower")]
        public async Task<ApiResponse<List<rank>>> GetPetPower()
        {
            var apiResponse = new ApiResponse<List<rank>>();

            try
            {
                var data = await _unitOfWork.Ranks.GetPetPowerRank();
                apiResponse.Success = true;
                apiResponse.Result = data;
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

        [HttpGet("Event")]
        public async Task<ApiResponse<List<rank>>> GetEvent()
        {
            var apiResponse = new ApiResponse<List<rank>>();

            try
            {
                var data = await _unitOfWork.Ranks.GetEvent();
                apiResponse.Success = true;
                apiResponse.Result = data;
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

        [HttpPost("Reward")]
        public async Task<ServiceResult> SendReward(int money)
        {
            ServiceResult result = new ServiceResult();

            result.Status = false;
            result.StatusMessage = "Có lỗi xảy ra ";

            try
            {
                int id = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);

                if (playerId == 0)
                {
                    result.Status = false;
                    result.StatusMessage = "Bạn chưa tạo nhân vật";
                    return result;
                }

                var data = await _unitOfWork.Ranks.GetEvent();
                result.Status = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = ex.Message;
            }

            return result;
        }

        private int GetAccountId()
        {
            // Lấy ClaimsPrincipal từ HttpContext
            var user = HttpContext.User;

            // Lấy claim từ ClaimsPrincipal
            int claimValue = int.Parse(user.FindFirst("id")?.Value);
            return claimValue;
        }
    }
}