using CleanArch.Application.Interfaces;
using Model;

namespace CleanArch.Api.Controllers
{
    public class ItemThreeController : BaseApiController<item3>
    {
        public ItemThreeController(IRepository<item3> repository) : base(repository)
        {
        }
    }
}
