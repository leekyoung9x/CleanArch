using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using CleanArch.Core.Utils;
using System.Text.Json;

namespace CleanArch.Api.Services
{
    public class CardService : ICardService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CardService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ChargingResponse> ChargingWS(ChargingRequest chargingRequest)
        {
            var baseURL = _configuration["Card:BaseURL"];
            var partnerID = _configuration["Card:PartnerID"];
            var partnerKey = _configuration["Card:PartnerKey"];
            var httpClient = _httpClientFactory.CreateClient();

            Random random = new Random();
            chargingRequest.RequestId = random.Next(0, Int32.MaxValue).ToString();

            chargingRequest.Sign = HashHelper.ComputeMd5Hash(partnerKey + chargingRequest.Code + chargingRequest.Serial);

            // Tạo nội dung yêu cầu
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("telco", chargingRequest.Telco),
                new KeyValuePair<string, string>("code", chargingRequest.Code),
                new KeyValuePair<string, string>("serial", chargingRequest.Serial),
                new KeyValuePair<string, string>("amount", chargingRequest.Amount),
                new KeyValuePair<string, string>("request_id", chargingRequest.RequestId),
                new KeyValuePair<string, string>("partner_id", partnerID),
                new KeyValuePair<string, string>("sign", chargingRequest.Sign),
                new KeyValuePair<string, string>("command", chargingRequest.Command)
            });

            var response = await httpClient.PostAsync($"{baseURL}/chargingws/v2", content);
            var jsonString = await response.Content.ReadAsStringAsync();

            // Deserialize JSON thành ReCaptchaResponse
            var reCaptchaResponse = JsonSerializer.Deserialize<ChargingResponse>(jsonString);

            return reCaptchaResponse;
        }
    }
}