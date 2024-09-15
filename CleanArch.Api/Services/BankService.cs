using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using System.Text.Json;

namespace CleanArch.Api.Services
{
    public class BankService : IBankService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public BankService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task<TransactionResponse> GetHistoryTransfer()
        {
            var result = new TransactionResponse();

            try
            {
                var baseURL = _configuration["Bank:BaseURL"];
                var action = _configuration["Bank:ACBAction"];
                var accountNumber = _configuration["Bank:AccountNumber"];
                var password = _configuration["Bank:Password"];
                var token = _configuration["Bank:Token"];

                var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.GetAsync($"{baseURL}/{action}/{password}/{accountNumber}/{token}");
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialize JSON thành ReCaptchaResponse
                result = JsonSerializer.Deserialize<TransactionResponse>(jsonString);

                try
                {
                    var elasticService = _serviceProvider.GetRequiredService<IElasticsearchService>();

                    elasticService.BulkUpsertDocuments<Transaction>(result.Transactions, doc => doc.TransactionID, "transaction_banking");
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}