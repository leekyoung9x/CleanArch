using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using CleanArch.Core.Entities.ResponseModel;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Api.Services;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public CardController(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // public CardController(IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(unitOfWork)
        // {
        //     _serviceProvider = serviceProvider;
        // }

        [HttpPost("chargingws")]
        public async Task<ApiResponse<ChargingResponse>> ChargingWS([FromBody]ChargingRequest chargingRequest)
        {
            var apiResponse = new ApiResponse<ChargingResponse>();
            
            try
            {
                var cardService = _serviceProvider.GetRequiredService<ICardService>();
                var data = await cardService.ChargingWS(chargingRequest);
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
