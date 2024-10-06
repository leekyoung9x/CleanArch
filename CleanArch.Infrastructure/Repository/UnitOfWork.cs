using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IAccountRepository accounts, IRankRepository ranks, ITransactionBanking transactionBanking, ITransactionCardRepository transactionCard, IPostRepository post)
        {
            Accounts = accounts;
            Ranks = ranks;
            TransactionBanking = transactionBanking;
            TransactionCard = transactionCard;
            Post = post;
        }

        public IAccountRepository Accounts { get; set; }
        public IRankRepository Ranks { get; set; }
        public ITransactionBanking TransactionBanking { get; set; }
        public ITransactionCardRepository TransactionCard { get; set; }
        public IPostRepository Post { get; set; }
    }
}