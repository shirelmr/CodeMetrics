using FluentValidation;
using MetricsAPI.DTOs;
using MetricsAPI.Models;
using MetricsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api/repositories")]
public class RepositoriesController : ControllerBase
{
    private readonly IRepositoryRepository _repo;
    private readonly IValidator<CreateRepositoryDto> _validator;

    public RepositoriesController(IRepositoryRepository repo, IValidator<CreateRepositoryDto> validator)
    {
        _repo = repo;
        _validator = validator;
    }
    
    // GET /api/repositories
    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<RepositoryResponseDto>>> GetAll([FromQuery] RepositoryQueryDto query)
    {
        if (query.PageSize > 50) query.PageSize = 50;
        if (query.Page < 1) query.Page = 1;

        var (items, totalCount) = await _repo.GetFilteredAsync(query);

        var response = new PagedResponseDto<RepositoryResponseDto>
        {
            Items = items.Select(repo => new RepositoryResponseDto
            {
                Id = repo.Id,
                Name = repo.Name,
                Url = repo.Url,
                Language = repo.Language
            }),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        return Ok(response);
    }

    // GET /api/repositories/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RepositoryResponseDto>> GetById(int id)
    {
        var repo = await _repo.GetByIdAsync(id);
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

        await _repo.AddAsync(repo);

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
        
        var updated = await _repo.UpdateAsync(new Repository
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
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }


}