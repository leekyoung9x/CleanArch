namespace CleanArch.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        IRankRepository Ranks { get; }
        ITransactionBanking TransactionBanking { get; }
        ITransactionCardRepository TransactionCard { get; }
        IPostRepository Post { get; }
        
        // Reward System Repositories
        IRewardPackageRepository RewardPackages { get; }
        ILeaderboardSeasonRepository LeaderboardSeasons { get; }
        IRewardPackageContentRepository RewardPackageContents { get; }
        IMilestoneRewardRepository MilestoneRewards { get; }
        IUserMilestoneClaimRepository UserMilestoneClaims { get; }
        ILeaderboardRewardRepository LeaderboardRewards { get; }
        IUserLeaderboardClaimRepository UserLeaderboardClaims { get; }
        
        // Item System Repositories
        IItemTemplateRepository ItemTemplates { get; }
        IItemOptionTemplateRepository ItemOptionTemplates { get; }
        
        // Gift Code Repository
        IGiftCodeRepository GiftCodes { get; }
    }
}