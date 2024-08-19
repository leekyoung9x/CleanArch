using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class AuthController : Controller
    {

        private readonly ITokenService _tokenService;
        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult NotAuthorized()
        {
            return Unauthorized();
        }

        [HttpPost]
        public async Task<string> Login()
        {
            try
            {
                return _tokenService.GenerateToken("haha");
            }
            catch (Exception ex)
            {
            }

            return "";
        }
    }
}
