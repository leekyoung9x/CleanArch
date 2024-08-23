namespace CleanArch.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        IRankRepository Ranks { get; }
        ITransactionBanking TransactionBanking { get; }
    }
}