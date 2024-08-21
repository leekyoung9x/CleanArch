using CleanArch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Application.Interfaces
{
    public interface IRankRepository : IRepository<rank>
    {
        Task<List<rank>> GetPowerRank();

        Task<List<rank>> GetPetPowerRank();
    }
}
