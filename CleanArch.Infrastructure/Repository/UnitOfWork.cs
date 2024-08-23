using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IAccountRepository accounts, IRankRepository ranks, ITransactionBanking transactionBanking)
        {
            Accounts = accounts;
            Ranks = ranks;
            TransactionBanking = transactionBanking;
        }

        public IAccountRepository Accounts { get; set; }
        public IRankRepository Ranks { get; set; }
        public ITransactionBanking TransactionBanking { get; set; }
    }
}