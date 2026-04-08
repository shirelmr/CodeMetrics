using Microsoft.AspNetCore.Mvc;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api")]
public class HealthController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "healthy" });
    }
}
