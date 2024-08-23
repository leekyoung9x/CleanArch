using CleanArch.Core.Entities.ResponseModel;
using System.Text;
using System.Text.Json;

namespace CleanArch.Api.Services
{
    public class BankService : IBankService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BankService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<TransactionResponse> GetBankHistory()
        {
            var accountNumber = _configuration["Banking:AccountNumber"];
            var password = _configuration["Banking:Password"];
            var token = _configuration["Banking:Token"];
            var baseURL = _configuration["Banking:BaseURL"];
            var actionBanking = _configuration["Banking:ActionBanking"];
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{baseURL}/{actionBanking}/{password}/{accountNumber}/{token}");
            var jsonString = await response.Content.ReadAsStringAsync();

            // Deserialize JSON thành ReCaptchaResponse
            var reCaptchaResponse = JsonSerializer.Deserialize<TransactionResponse>(jsonString);

            return reCaptchaResponse;
        }
    }
}