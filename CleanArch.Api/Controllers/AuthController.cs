using CleanArch.Api.Filter;
using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut("Password")]
        public async Task<ServiceResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(model.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                var authService = _serviceProvider.GetRequiredService<IAuthService>();
                var accountRepository = _serviceProvider.GetRequiredService<IAccountRepository>();

                account? account = await GetAccountByToken(accountRepository);

                if (account.password == model.password)
                {
                    var success = await accountRepository.ChangePassword(account.id, model.passwordnew);
                    if (success)
                    {
                        result.Status = true;
                        result.StatusMessage = "Đổi mật khẩu thành công";
                    }
                    else
                    {
                        result.Status = false;
                        result.StatusMessage = "Đổi mật khẩu thất bại";
                    }
                }
                else
                {
                    result.Status = false;
                    result.StatusMessage = "Mật khẩu hiện tại không chính xác";
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

        private async Task<account> GetAccountByToken(IAccountRepository accountRepository)
        {
            // Lấy ClaimsPrincipal từ HttpContext
            var user = HttpContext.User;

            // Lấy claim từ ClaimsPrincipal
            int claimValue = int.Parse(user.FindFirst("id")?.Value);
            var account = await accountRepository.GetByIdAsync(claimValue);

            return account;
        }

        [HttpPost("Register")]
        public async Task<ServiceResult> Register([FromBody] LoginModel model)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(model.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                var authService = _serviceProvider.GetRequiredService<IAuthService>();
                var accountRepository = _serviceProvider.GetRequiredService<IAccountRepository>();

                var isExist = await accountRepository.IsExist(model.username);

                if (isExist)
                {
                    result.Status = false;
                    result.StatusMessage = "Tài khoản đã tồn tại";
                    return result;
                }

                // Lấy địa chỉ IP của máy gửi request
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var isSucess = await accountRepository.Register(model.username, model.password, ipAddress);

                if (isSucess)
                {
                    result.Status = true;
                    result.StatusMessage = "Đăng ký thành công";
                }
                else
                {
                    result.Status = false;
                    result.StatusMessage = "Đăng ký thất bại";
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

        [HttpGet()]
        [Authorize]
        public async Task<ServiceResult> GetByID()
        {
            ServiceResult result = new ServiceResult();
            var accountRepository = _serviceProvider.GetRequiredService<IAccountRepository>();

            // Lấy ClaimsPrincipal từ HttpContext
            var user = HttpContext.User;

            // Lấy claim từ ClaimsPrincipal
            var claimValue = int.Parse(user.FindFirst("id")?.Value);
            var account = await accountRepository.GetByIdAsync(claimValue);

            result.Status = true;
            result.Data = new
            {
                vnd = account.vnd,
                role = account.role,
                active = account.active,
            };

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