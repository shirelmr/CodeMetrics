using FluentValidation;
using MetricsAPI.DTOs;
using MetricsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api/repositories")]
public class RepositoriesController : ControllerBase
{
    // In-memory storage — a simple list acting as your "database" for now
    private static readonly List<Repository> _repositories = new();
    private static int _nextId = 1;

    public static int GetCount() => _repositories.Count;

    private readonly IValidator<CreateRepositoryDto> _validator;

    // Constructor — .NET automatically injects the validator here
    public RepositoriesController(IValidator<CreateRepositoryDto> validator)
    {
        _validator = validator;
    }

    // GET /api/repositories
    [HttpGet]
    public ActionResult<IEnumerable<RepositoryResponseDto>> GetAll()
    {
        var result = _repositories.Select(r => new RepositoryResponseDto
        {
            Id = r.Id,
            Name = r.Name,
            Url = r.Url,
            Language = r.Language
        });

        return Ok(result);
    }

    // GET /api/repositories/{id}
    [HttpGet("{id}")]
    public ActionResult<RepositoryResponseDto> GetById(int id)
    {
        var repo = _repositories.FirstOrDefault(r => r.Id == id);
        if (repo is null) return NotFound();

        return Ok(new RepositoryResponseDto
        {
            Id = repo.Id,
            Name = repo.Name,
            Url = repo.Url,
            Language = repo.Language
        });
    }

    // POST /api/repositories
    [HttpPost]
    public async Task<ActionResult<RepositoryResponseDto>> Create(CreateRepositoryDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var repo = new Repository
        {
            Id = _nextId++,
            Name = dto.Name,
            Url = dto.Url,
            Language = dto.Language
        };

        _repositories.Add(repo);

        var response = new RepositoryResponseDto
        {
            Id = repo.Id,
            Name = repo.Name,
            Url = repo.Url,
            Language = repo.Language
        };

        return CreatedAtAction(nameof(GetById), new { id = repo.Id }, response);
    }
}