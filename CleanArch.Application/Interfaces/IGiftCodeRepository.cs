using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IGiftCodeRepository : IRepository<GiftCode>
    {
        Task<string> GenerateUniqueGiftCodeAsync();
        Task<bool> IsGiftCodeExistsAsync(string code);
        Task<long> AddAndReturnIdAsync(GiftCode giftCode);
    }
}
