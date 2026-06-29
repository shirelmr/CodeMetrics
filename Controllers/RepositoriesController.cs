using AutoMapper;
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
    private readonly ILogger<RepositoriesController> _logger;
    private readonly IMapper _mapper;

    public RepositoriesController(IRepositoryRepository repo, IValidator<CreateRepositoryDto> validator, ILogger<RepositoriesController> logger, IMapper mapper)
    {
        _repo = repo;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }
    
    // GET /api/repositories
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<RepositoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponseDto<RepositoryResponseDto>>> GetAll([FromQuery] RepositoryQueryDto query)
    {
        try
        {
            if (query.PageSize > 50) query.PageSize = 50;
            if (query.Page < 1) query.Page = 1;

            var (items, totalCount) = await _repo.GetFilteredAsync(query);

            _logger.LogInformation("Retrieved {Count} repositories (Page {Page}, PageSize {PageSize})", items.Count(), query.Page, query.PageSize);
            return Ok(_mapper.Map<IEnumerable<RepositoryResponseDto>>(items));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving repositories");
            return StatusCode(500, "An error occurred while retrieving repositories");
        }
    }

    // GET /api/repositories/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RepositoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepositoryResponseDto>> GetById(int id)
    {
        try
        {
            var repo = await _repo.GetByIdAsync(id);
            if (repo is null)
            {
                _logger.LogWarning("Repository with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Retrieved repository with ID {Id}", id);    
            return Ok(_mapper.Map<RepositoryResponseDto>(repo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving repository with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the repository");
        }
    }

    // POST /api/repositories
    [HttpPost]
    [ProducesResponseType(typeof(RepositoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RepositoryResponseDto>> Create(CreateRepositoryDto dto)
    {
        try
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for repository creation: {Errors}", 
                    string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
            }

            var repo = _mapper.Map<Repository>(dto);

            await _repo.AddAsync(repo);

            var response = _mapper.Map<RepositoryResponseDto>(repo);

            _logger.LogInformation("Created repository with ID {Id} and name {Name}", repo.Id, repo.Name);
            return CreatedAtAction(nameof(GetById), new { id = repo.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating repository: {Name}", dto.Name);
            return StatusCode(500, "An error occurred while creating the repository");
        }
    }

    // PUT /api/repositories/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RepositoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepositoryResponseDto>> Update(int id, CreateRepositoryDto dto)
    {
        try
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for repository update (ID {Id}): {Errors}", 
                    id, string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
            }
            
            var updated = await _repo.UpdateAsync(_mapper.Map<Repository>(dto));
            
            if (!updated)
            {
                _logger.LogWarning("Repository with ID {Id} not found for update", id);
                return NotFound();
            }
            
            _logger.LogInformation("Updated repository with ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating repository with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the repository");
        }
    }

    // DELETE /api/repositories/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Repository with ID {Id} not found for deletion", id);
                return NotFound();
            }
            
            _logger.LogInformation("Deleted repository with ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting repository with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the repository");
        }
    }


}