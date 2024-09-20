namespace CleanArch.Api.Services
{
    public interface IRankService
    {
        Task<bool> GetReward(int money, int playerId);
    }
}
