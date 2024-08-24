using CleanArch.Api.Models;
using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : BaseApiController
    {
        private readonly IServiceProvider _serviceProvider;

        public BankController(IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(unitOfWork)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("GetBankHistory")]
        public async Task<ApiResponse<TransactionResponse>> GetBankHistory()
        {
            var apiResponse = new ApiResponse<TransactionResponse>();

            try
            {
                var bankService = _serviceProvider.GetRequiredService<IBankService>();
                var data = await bankService.GetHistoryTransfer();
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