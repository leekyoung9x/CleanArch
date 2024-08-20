namespace CleanArch.Api.Services
{
    public interface IAuthService
    {
        string GenerateToken(string userName);
    }
}