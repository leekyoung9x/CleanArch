using CleanArch.Api.Models;
using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public RankController(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
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

        [HttpGet("Reward")]
        public async Task<ServiceResult> SendReward([FromQuery] int amount)
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

                var accum = await _unitOfWork.Accounts.GetPlayerAccumulateByPlayerId(playerId);

                if (accum < amount)
                {
                    result.Status = false;
                    result.StatusMessage = "Bạn chưa nạp đủ mốc " + amount;
                    return result;
                }

                var haveDone = await _unitOfWork.Accounts.GetPlayerAccumulateHaveDoneByPlayerId(playerId, amount);

                if (haveDone > 0)
                {
                    result.Status = false;
                    result.StatusMessage = "Bạn đã nhận quà mốc " + amount;
                    return result;
                }

                var rankService = _serviceProvider.GetRequiredService<IRankService>();

                var data = await rankService.GetReward(amount, playerId);
                result.StatusMessage = "Đang xử lý hộp quà của bạn";
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