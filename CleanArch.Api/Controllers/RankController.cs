using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

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
    }
}