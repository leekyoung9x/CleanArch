namespace CleanArch.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IContactRepository Contacts { get; }
        IItemThreeRepository ItemThree { get; }
    }
}
