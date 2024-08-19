namespace CleanArch.Api.Services
{
    public interface IReCaptchaService
    {
        Task<bool> VerifyTokenAsync(string token);
    }
}
