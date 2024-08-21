namespace CleanArch.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IContactRepository Contacts { get; }
        IRankRepository Ranks { get; }
    }
}