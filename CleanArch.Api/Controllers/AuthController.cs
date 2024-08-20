using CleanArch.Api.Filter;
using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public AuthController(IServiceProvider serviceProvider, IAuthService authService, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<ServiceResult> Login([FromBody] LoginModel model)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(model.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                var authService = _serviceProvider.GetRequiredService<IAuthService>();
                var accountRepository = _serviceProvider.GetRequiredService<IAccountRepository>();

                var account = await accountRepository.Login(model.username, model.password);

                if (account != null)
                {
                    result.Status = true;
                    result.Data = authService.GenerateToken(account);
                    result.StatusMessage = "Đăng nhập thành công";
                }
                else
                {
                    result.Status = false;
                    result.StatusMessage = "Tài khoản và mật khẩu không chính xác";
                }

                
            } 
            else
            {
                result.Status = false;
                result.StatusMessage = "Captcha không hợp lệ";
            }

            // Handle form submission
            return result;
        }

        [HttpPost("CheckToken")]
        public IActionResult CheckToken([FromQuery] string token)
        {
            string result = "";

            var tokenValidator = new TokenValidator(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"]);

            var principal = tokenValidator.ValidateToken(token);
            if (principal != null)
            {
                result = "Token is valid.";
            }
            else
            {
                result = "Token is invalid.";
            }
            return Ok(result);
        }
    }
}