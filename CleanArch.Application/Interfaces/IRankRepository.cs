using CleanArch.Core.Entities;

namespace CleanArch.Application.Interfaces
{
    public interface IRankRepository : IRepository<rank>
    {
        Task<List<rank>> GetPowerRank();

        Task<List<rank>> GetPetPowerRank();

        Task<List<rank>> GetVndRank();
    }
}