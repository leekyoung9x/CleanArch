using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(
            IAccountRepository accounts, 
            IRankRepository ranks, 
            ITransactionBanking transactionBanking, 
            ITransactionCardRepository transactionCard, 
            IPostRepository post,
            IRewardPackageRepository rewardPackages,
            ILeaderboardSeasonRepository leaderboardSeasons,
            IRewardPackageContentRepository rewardPackageContents,
            IMilestoneRewardRepository milestoneRewards,
            IUserMilestoneClaimRepository userMilestoneClaims,
            ILeaderboardRewardRepository leaderboardRewards,
            IUserLeaderboardClaimRepository userLeaderboardClaims,
            IItemTemplateRepository itemTemplates,
            IItemOptionTemplateRepository itemOptionTemplates,
            IGiftCodeRepository giftCodes)
        {
            Accounts = accounts;
            Ranks = ranks;
            TransactionBanking = transactionBanking;
            TransactionCard = transactionCard;
            Post = post;
            RewardPackages = rewardPackages;
            LeaderboardSeasons = leaderboardSeasons;
            RewardPackageContents = rewardPackageContents;
            MilestoneRewards = milestoneRewards;
            UserMilestoneClaims = userMilestoneClaims;
            LeaderboardRewards = leaderboardRewards;
            UserLeaderboardClaims = userLeaderboardClaims;
            ItemTemplates = itemTemplates;
            ItemOptionTemplates = itemOptionTemplates;
            GiftCodes = giftCodes;
        }

        public IAccountRepository Accounts { get; set; }
        public IRankRepository Ranks { get; set; }
        public ITransactionBanking TransactionBanking { get; set; }
        public ITransactionCardRepository TransactionCard { get; set; }
        public IPostRepository Post { get; set; }

        // Reward System Repositories
        public IRewardPackageRepository RewardPackages { get; set; }
        public ILeaderboardSeasonRepository LeaderboardSeasons { get; set; }
        public IRewardPackageContentRepository RewardPackageContents { get; set; }
        public IMilestoneRewardRepository MilestoneRewards { get; set; }
        public IUserMilestoneClaimRepository UserMilestoneClaims { get; set; }
        public ILeaderboardRewardRepository LeaderboardRewards { get; set; }
        public IUserLeaderboardClaimRepository UserLeaderboardClaims { get; set; }
        
        // Item System Repositories
        public IItemTemplateRepository ItemTemplates { get; set; }
        public IItemOptionTemplateRepository ItemOptionTemplates { get; set; }
        
        // Gift Code Repository
        public IGiftCodeRepository GiftCodes { get; set; }
    }
}