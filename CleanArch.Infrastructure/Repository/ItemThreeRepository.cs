using CleanArch.Application.Interfaces;
using CleanArch.Infrastructure.Base;
using Microsoft.Extensions.Configuration;
using Model;

namespace CleanArch.Infrastructure.Repository
{
    public class ItemThreeRepository : BaseRepository<item3>, IItemThreeRepository
    {
        public ItemThreeRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
