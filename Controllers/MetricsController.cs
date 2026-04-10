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
            totalRepositories = RepositoriesController.GetCount(),
            totalMetrics = 0,
            lastUpdated = DateTime.UtcNow
        });
    }
}