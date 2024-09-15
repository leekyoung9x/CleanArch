using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using System.Text.Json;
using System.Text;

namespace CleanArch.Api.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CaptchaService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
            var secretKey = _configuration["Captcha:SecretKey"];
            var apiKey = _configuration["Captcha:APIKey"];
            var httpClient = _httpClientFactory.CreateClient();

            var values = new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", token }
            };

            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var captchaResult = JsonSerializer.Deserialize<TurnstileResponse>(responseString);

            return captchaResult?.Success ?? false;
        }
    }
}
