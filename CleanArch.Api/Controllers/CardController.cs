using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities.Constant;
using CleanArch.Core.Entities.Enumeration;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

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
        public async Task<ServiceResult> ChargingWS([FromBody] ChargingRequest chargingRequest)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(chargingRequest.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                var cardService = _serviceProvider.GetRequiredService<ICardService>();
                var data = await cardService.ChargingWS(chargingRequest);

                switch (data.Status)
                {
                    case (int)CardType.Success:
                        result.StatusMessage = CardTypeName.Success;
                        break;
                    case (int)CardType.SuccessHalf:
                        result.StatusMessage = CardTypeName.SuccessHalf;
                        break;
                    case (int)CardType.Error:
                        result.StatusMessage = CardTypeName.Error;
                        break;
                    case (int)CardType.Maintain:
                        result.StatusMessage = CardTypeName.Maintain;
                        break;
                    case (int)CardType.Pending:
                        result.StatusMessage = CardTypeName.Pending;
                        break;
                    case (int)CardType.SendFail:
                        result.StatusMessage = CardTypeName.SendFail;
                        break;
                    default:
                        break;
                }

                result.Status = data.Status == (int)CardType.Success || data.Status == (int)CardType.SuccessHalf ? true : false;
                result.Data = data;
            }
            else
            {
                result.Status = false;
                result.StatusMessage = "Captcha không hợp lệ";
            }

            // Handle form submission
            return result;
        }
    }
}