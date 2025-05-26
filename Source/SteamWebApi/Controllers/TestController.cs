using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is working!");
        }

        [HttpGet("cors")]
        public IActionResult TestCors()
        {
            // Adding CORS-related headers for diagnostics
            Response.Headers.Add("X-CORS-TEST", "This header should be visible if CORS is working");
            return Ok(new { message = "CORS test endpoint", timestamp = DateTime.Now });
        }

        [HttpGet("echo")]
        public IActionResult Echo([FromQuery] string message)
        {
            return Ok(new
            {
                message = string.IsNullOrEmpty(message) ? "No message provided" : message,
                receivedAt = DateTime.Now
            });
        }

        [HttpGet("error")]
        public IActionResult TestError()
        {
            try
            {
                // Deliberately cause an error
                throw new Exception("This is a test exception");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}