using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Api.Services
{
    public interface ICaptchaService
    {
        Task<bool> VerifyTokenAsync(string token);
    }
}
