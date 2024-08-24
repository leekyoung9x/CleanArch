using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface ITransactionBanking : IRepository<transaction_banking>
    {
        Task<transaction_banking?> GetTransactionByCustom(int id, int amount, string? otp);

        Task<List<transaction_banking>> GetTransactionBankingByPlayerId(int id);
    }
}