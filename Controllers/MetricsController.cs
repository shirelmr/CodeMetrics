using Microsoft.AspNetCore.Mvc;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        return Ok(new
        {
            totalRepositories = 0,
            totalMetrics = 0,
            lastUpdated = DateTime.UtcNow
        });
    }
}