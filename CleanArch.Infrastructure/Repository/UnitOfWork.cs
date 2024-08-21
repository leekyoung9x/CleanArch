using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IContactRepository contactRepository, IAccountRepository accounts, IRankRepository ranks)
        {
            Contacts = contactRepository;
            Accounts = accounts;
            Ranks = ranks;
        }

        public IContactRepository Contacts { get; set; }
        public IAccountRepository Accounts { get; set; }
        public IRankRepository Ranks { get; set; }
    }
}