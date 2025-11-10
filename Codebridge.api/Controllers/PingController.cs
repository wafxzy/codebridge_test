using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codebridge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet("ping")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok("Dogshouseservice.Version1.0.1");
        }
    }
}
