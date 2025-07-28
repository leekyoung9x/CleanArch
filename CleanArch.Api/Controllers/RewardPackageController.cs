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
    public class RewardPackageController : BaseApiController
    {
        public RewardPackageController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<RewardPackage>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<RewardPackage>>();

            try
            {
                var data = await _unitOfWork.RewardPackages.GetAllAsync();
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
        public async Task<ApiResponse<RewardPackage>> GetById(int id)
        {
            var apiResponse = new ApiResponse<RewardPackage>();

            try
            {
                var data = await _unitOfWork.RewardPackages.GetByIdAsync(id);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Reward package not found";
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

        [HttpGet("{id}/contents")]
        [AllowAnonymous]
        public async Task<ApiResponse<RewardPackage>> GetByIdWithContents(int id)
        {
            var apiResponse = new ApiResponse<RewardPackage>();

            try
            {
                var data = await _unitOfWork.RewardPackages.GetByIdWithContentsAsync(id);
                if (data != null)
                {
                    apiResponse.Success = true;
                    apiResponse.Result = data;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Reward package not found";
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
        public async Task<ApiResponse<bool>> Create([FromBody] RewardPackage rewardPackage)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.RewardPackages.AddAsync(rewardPackage);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Reward package created successfully" : "Failed to create reward package";
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
        public async Task<ApiResponse<bool>> Update([FromBody] RewardPackage rewardPackage)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.RewardPackages.UpdateAsync(rewardPackage);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Reward package updated successfully" : "Failed to update reward package";
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
                var result = await _unitOfWork.RewardPackages.DeleteAsync(id);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Reward package deleted successfully" : "Failed to delete reward package";
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
