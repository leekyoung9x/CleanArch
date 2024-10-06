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
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IRankRepository, RankRepository>();
            services.AddTransient<ITransactionBanking, TransactionBanking>();
            services.AddTransient<ITransactionCardRepository, TransactionCardRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}