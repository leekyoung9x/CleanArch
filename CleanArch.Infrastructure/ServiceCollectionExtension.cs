using CleanArch.Application.Interfaces;
using CleanArch.Infrastructure.Base;
using CleanArch.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Initialize Dapper Type Mappings for all entities
            DapperTypeMapper.SetupEntityMappings();
            
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IRankRepository, RankRepository>();
            services.AddTransient<ITransactionBanking, TransactionBanking>();
            services.AddTransient<ITransactionCardRepository, TransactionCardRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            
            // Reward System Repositories
            services.AddTransient<IRewardPackageRepository, RewardPackageRepository>();
            services.AddTransient<ILeaderboardSeasonRepository, LeaderboardSeasonRepository>();
            services.AddTransient<IRewardPackageContentRepository, RewardPackageContentRepository>();
            services.AddTransient<IMilestoneRewardRepository, MilestoneRewardRepository>();
            services.AddTransient<IUserMilestoneClaimRepository, UserMilestoneClaimRepository>();
            services.AddTransient<ILeaderboardRewardRepository, LeaderboardRewardRepository>();
            services.AddTransient<IUserLeaderboardClaimRepository, UserLeaderboardClaimRepository>();
            
            // Item System Repositories
            services.AddTransient<IItemTemplateRepository, ItemTemplateRepository>();
            services.AddTransient<IItemOptionTemplateRepository, ItemOptionTemplateRepository>();
            
            // Gift Code Repository
            services.AddTransient<IGiftCodeRepository, GiftCodeRepository>();
            
            services.AddTransient(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}