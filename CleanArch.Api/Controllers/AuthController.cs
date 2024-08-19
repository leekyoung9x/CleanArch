using CleanArch.Api.Filter;
using CleanArch.Api.Services;
using CleanArch.Core.Entities.RequestModel;
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
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var isCaptchaValid = await reCaptchaService.VerifyTokenAsync(model.token);
            if (!isCaptchaValid)
            {
                return BadRequest("reCAPTCHA validation failed");
            }

            var authService = _serviceProvider.GetRequiredService<IAuthService>();

            // Handle form submission
            return Ok(authService.GenerateToken("admin"));
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
