using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Api.Services
{
    public interface IBankService
    {
        Task<TransactionResponse> GetHistoryTransfer();
    }
}