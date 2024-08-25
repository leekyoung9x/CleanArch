using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IAccountRepository : IRepository<account>
    {
        Task<account?> Login(string username, string password);

        Task<bool> IsExist(string username);

        Task<bool> Register(string username, string password, string ip);

        Task<bool> ChangePassword(int id, string passwordNew);

        Task<int> GetPlayerIdByAccountId(int id);

        Task<int> GetPlayerAccumulateByPlayerId(int id);
        Task<int> GetAccountIdByPlayerId(int id);
    }
}