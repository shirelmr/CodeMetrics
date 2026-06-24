using MetricsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    private readonly RepositoryStore _store;

    public MetricsController(RepositoryStore store)
    {
        _store = store;
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        return Ok(new
        {
            totalRepositories = _store.Count(),
            totalMetrics = 0,
            lastUpdated = DateTime.UtcNow
        });
    }
}