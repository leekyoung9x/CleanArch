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

        public async Task<bool> VerifyTokenAsync(string token)
        {
            var secretKey = _configuration["ReCaptcha:SecretKey"];
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);
            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(jsonString);

            if (jsonResponse.RootElement.GetProperty("success").GetBoolean())
            {
                var score = jsonResponse.RootElement.GetProperty("score").GetDouble();
                return score > 0.5; // Chỉ chấp nhận nếu score > 0.5
            }
            return false;
        }
    }
}
