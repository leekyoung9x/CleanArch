using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Microsoft.Extensions.Configuration;

namespace CleanArch.Infrastructure.Repository
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
