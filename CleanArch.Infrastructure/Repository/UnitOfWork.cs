using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IContactRepository contactRepository, IAccountRepository accounts)
        {
            Contacts = contactRepository;
            Accounts = accounts;
        }

        public IContactRepository Contacts { get; set; }
        public IAccountRepository Accounts { get; set; }
    }
}