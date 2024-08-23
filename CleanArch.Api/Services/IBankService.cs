using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Api.Services
{
    public interface IBankService
    {
        Task<TransactionResponse> GetBankHistory();
    }
}