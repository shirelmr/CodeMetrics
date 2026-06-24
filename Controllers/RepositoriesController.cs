using FluentValidation;
using MetricsAPI.DTOs;
using MetricsAPI.Models;
using MetricsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api/repositories")]
public class RepositoriesController : ControllerBase
{
    private readonly RepositoryStore _store;
    private readonly IValidator<CreateRepositoryDto> _validator;

    public RepositoriesController(RepositoryStore store, IValidator<CreateRepositoryDto> validator)
    {
        _store = store;
        _validator = validator;
    }
    
    // GET /api/repositories
    [HttpGet]
    public ActionResult<IEnumerable<RepositoryResponseDto>> GetAll()
    {
        var result = _store.GetAll().Select(r => new RepositoryResponseDto
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
        var repo = _store.GetById(id);
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
            Name = dto.Name,
            Url = dto.Url,
            Language = dto.Language
        };

        _store.Add(repo);

        var response = new RepositoryResponseDto
        {
            Id = repo.Id,
            Name = repo.Name,
            Url = repo.Url,
            Language = repo.Language
        };

        return CreatedAtAction(nameof(GetById), new { id = repo.Id }, response);
    }

    // PUT /api/repositories/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RepositoryResponseDto>> Update(int id, CreateRepositoryDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        
        var updated = _store.Update(new Repository
        {
            Id = id,
            Name = dto.Name,
            Url = dto.Url,
            Language = dto.Language
        });
        if (!updated) return NotFound();
        return NoContent();

    }

    // DELETE /api/repositories/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _store.Delete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }


}