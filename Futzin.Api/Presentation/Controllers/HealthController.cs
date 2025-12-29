using Microsoft.AspNetCore.Mvc;

namespace Futzin.Api.Presentation.Controllers;

[ApiController]
[Route("api")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello World", status = "API is running" });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "Futzin API"
        });
    }
}
