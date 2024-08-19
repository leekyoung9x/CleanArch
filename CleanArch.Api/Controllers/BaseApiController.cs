using CleanArch.Api.Filter;
using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(AuthorizationFilterAttribute))]
    [ApiController]
    [Authorize]
    public class BaseApiController<T> : ControllerBase where T : class
    {
        private readonly IRepository<T> Repository;

        /// <summary>
        /// 
        /// </summary>
        public BaseApiController()
        {
            
        }

        public BaseApiController(IRepository<T> repository)
        {
            Repository = repository;
        }

        [HttpGet]
        public virtual async Task<ApiResponse<List<T>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<T>>();

            try
            {
                var data = await Repository.GetAllAsync();
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

        [HttpGet("GetPaging")]
        public virtual async Task<ApiResponse<List<T>>> GetPaging()
        {
            var apiResponse = new ApiResponse<List<T>>();

            try
            {
                var data = await Repository.GetAllAsync();
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
    }
}