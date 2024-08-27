using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities.Constant;
using CleanArch.Core.Entities.Enumeration;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : BaseApiController
    {
        private readonly IServiceProvider _serviceProvider;

        //public CardController(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        //{
        //    _serviceProvider = serviceProvider;
        //}

        public CardController(IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(unitOfWork)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("chargingws")]
        public async Task<ServiceResult> ChargingWS([FromBody] ChargingRequest chargingRequest)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(chargingRequest.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                int id = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);

                if (playerId == 0)
                {
                    result.Status = false;
                    result.StatusMessage = "Bạn chưa tạo nhân vật";
                    return result;
                }
                
                var cardService = _serviceProvider.GetRequiredService<ICardService>();
                var data = await cardService.ChargingWS(chargingRequest, _unitOfWork, id);

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

        [AllowAnonymous]
        [HttpPost("callback")]
        public async Task<ServiceResult> Callback([FromBody] CallbackRequest model)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                var elasticService = _serviceProvider.GetRequiredService<IElasticsearchService>();

                var elk = await elasticService.InsertDataELK<CallbackRequest>(new List<CallbackRequest>() {model});

                result.Data = elk.DebugInformation;

            }
            catch (Exception e)
            {
                result.Data = e;
            }

            var cardService = _serviceProvider.GetRequiredService<ICardService>();
            await cardService.HandleCallback(model, _unitOfWork);
            result.Status = true;

            return result;
        }

        [HttpGet("history")]
        public async Task<ServiceResult> GetHistory()
        {
            ServiceResult result = new ServiceResult();

            int id = GetAccountId();
            int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);
            var data = await _unitOfWork.TransactionCard.GetByIdAsync("player_id", playerId);

            List<TransactionCardResponse> reponse = new List<TransactionCardResponse>();

            foreach (var item in data)
            {
                reponse.Add(new TransactionCardResponse()
                {
                    request_id = item.request_id,
                    amount = item.amount,
                    amount_real = item.amount_real,
                    code = item.code,
                    seri = item.seri,
                    card_type = item.card_type,
                    status = item.status,
                    time = item.time,
                });
            }

            result.Status = true;
            result.Data = reponse.OrderByDescending(n => n.time);

            // Handle form submission
            return result;
        }

        [HttpPost("check")]
        public async Task<ServiceResult> CheckTransaction([FromBody] CheckRequestModel model)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(model.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                int id = GetAccountId();
                var cardService = _serviceProvider.GetRequiredService<ICardService>();
                var data = await cardService.CheckTransaction(model, _unitOfWork, id);

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