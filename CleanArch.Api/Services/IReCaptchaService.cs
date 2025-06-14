using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Api.Services
{
    public interface IReCaptchaService
    {
        Task<ResponseCaptcha> VerifyTokenAsync(string token);
        Task<bool> IsValidCaptchaAsync(string token);
    }
}
