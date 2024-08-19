using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IContactRepository contactRepository, IItemThreeRepository itemThree)
        {
            Contacts = contactRepository;
            ItemThree = itemThree;
        }

        public IContactRepository Contacts { get; set; }

        public IItemThreeRepository ItemThree { get; set; }
    }
}
