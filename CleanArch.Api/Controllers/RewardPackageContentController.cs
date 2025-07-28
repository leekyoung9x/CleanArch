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
    public class RewardPackageContentController : BaseApiController
    {
        public RewardPackageContentController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<RewardPackageContent>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<RewardPackageContent>>();

            try
            {
                var data = await _unitOfWork.RewardPackageContents.GetAllAsync();
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

        [HttpGet("package/{packageId}")]
        [AllowAnonymous]
        public async Task<ApiResponse<List<RewardPackageContent>>> GetByPackageId(int packageId)
        {
            var apiResponse = new ApiResponse<List<RewardPackageContent>>();

            try
            {
                var data = await _unitOfWork.RewardPackageContents.GetContentsByPackageIdAsync(packageId);
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
        public async Task<ApiResponse<bool>> Create([FromBody] RewardPackageContent content)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.RewardPackageContents.AddAsync(content);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Package content created successfully" : "Failed to create package content";
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
        public async Task<ApiResponse<bool>> Update([FromBody] RewardPackageContent content)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.RewardPackageContents.UpdateAsync(content);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Package content updated successfully" : "Failed to update package content";
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

        [HttpDelete("package/{packageId}")]
        public async Task<ApiResponse<bool>> DeleteByPackageId(int packageId)
        {
            var apiResponse = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.RewardPackageContents.DeleteByPackageIdAsync(packageId);
                apiResponse.Success = result;
                apiResponse.Result = result;
                apiResponse.Message = result ? "Package contents deleted successfully" : "Failed to delete package contents";
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
