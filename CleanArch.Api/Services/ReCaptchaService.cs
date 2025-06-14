using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using System.Text;
using System.Text.Json;

namespace CleanArch.Api.Services
{
    public class ReCaptchaService : IReCaptchaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ReCaptchaService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ResponseCaptcha> VerifyTokenAsync(string token)
        {
            var siteKey = _configuration["ReCaptcha:SiteKey"];
            var apiKey = _configuration["ReCaptcha:APIKey"];
            var httpClient = _httpClientFactory.CreateClient();

            var recaptchaRequest = new RecaptchaRequest
            {
                Event = new EventData
                {
                    Token = token,
                    ExpectedAction = "Login",
                    SiteKey = siteKey
                }
            };

            var json = JsonSerializer.Serialize(recaptchaRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"https://recaptchaenterprise.googleapis.com/v1/projects/dragonboy-a56f7/assessments?key={apiKey}", jsonContent);
            var jsonString = await response.Content.ReadAsStringAsync();

            // Deserialize JSON thành ReCaptchaResponse
            var reCaptchaResponse = JsonSerializer.Deserialize<ResponseCaptcha>(jsonString);

            return reCaptchaResponse;
        }

        public async Task<bool> IsValidCaptchaAsync(string token)
        {
            var response = await VerifyTokenAsync(token);
            var minimumScore = _configuration.GetValue<float>("ReCaptcha:MinimumScore");
            
            return response != null && 
                   response.TokenProperties?.Valid == true && 
                   response.RiskAnalysis?.Score >= minimumScore;
        }
    }
}
