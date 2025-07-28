using CleanArch.Api.Util;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KeyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetKey()
        {
            return Ok(SecurityAES.Encrypt(_configuration["ClientKey:Key"], _configuration["ClientKey:AES"]));
        }

        [HttpGet("pta")]
        public IActionResult GetAES()
        {
            return Ok(_configuration["ClientKey:AES"]);
        }
    }
}
