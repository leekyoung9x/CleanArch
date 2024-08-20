using CleanArch.Core.Entities;

namespace CleanArch.Api.Services
{
    public interface IAuthService
    {
        string GenerateToken(account model);
    }
}