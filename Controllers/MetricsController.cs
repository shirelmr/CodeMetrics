using Microsoft.AspNetCore.Mvc;
using MetricsAPI.Repositories;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    private readonly IRepositoryRepository _repo;

    public MetricsController(IRepositoryRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var totalRepositories = (await _repo.GetAllAsync()).Count();
        return Ok(new
        {
            totalRepositories = totalRepositories,
            totalMetrics = 0,
            lastUpdated = DateTime.UtcNow
        });
    }
}