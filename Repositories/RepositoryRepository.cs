using MetricsAPI.Data;
using MetricsAPI.Models;
using MetricsAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MetricsAPI.Repositories;

public class RepositoryRepository : IRepositoryRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<RepositoryRepository> _logger;

    public RepositoryRepository(AppDbContext context, ILogger<RepositoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Repository>> GetAllAsync()
    {
        return await _context.Repositories.ToListAsync();
    }

    public async Task<Repository?> GetByIdAsync(int id, int userId)
    {
        return await _context.Repositories
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
    }

    public async Task<Repository> AddAsync(Repository repo)
    {
        try
        {
            repo.CreatedAt = DateTime.UtcNow;
            _context.Repositories.Add(repo);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Repository {Name} added to database with ID {Id}", repo.Name, repo.Id);
            return repo;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error adding repository {Name}", repo.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Repository repo, int userId)
    {
        try
        {
            var existing = await _context.Repositories
                .FirstOrDefaultAsync(r => r.Id == repo.Id && r.UserId == userId);
            if (existing is null)
            {
                _logger.LogDebug("Repository with ID {Id} not found for update", repo.Id);
                return false;
            }

            existing.Name = repo.Name;
            existing.Url = repo.Url;
            existing.Language = repo.Language;

            await _context.SaveChangesAsync();
            _logger.LogDebug("Repository with ID {Id} updated in database", repo.Id);
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating repository with ID {Id}", repo.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var repo = await _context.Repositories.FindAsync(id);
            if (repo is null)
            {
                _logger.LogDebug("Repository with ID {Id} not found for deletion", id);
                return false;
            }

            _context.Repositories.Remove(repo);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Repository with ID {Id} deleted from database", id);
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting repository with ID {Id}", id);
            throw;
        }
    }

    public async Task<int> CountAsync()
    {
        return await _context.Repositories.CountAsync();
    }

    public async Task<(IEnumerable<Repository> Items, int TotalCount)> GetFilteredAsync(RepositoryQueryDto query, int userId)
    {
        _logger.LogDebug("Filtering repositories: Language={Language}, Sort={Sort}, Page={Page}, PageSize={PageSize}", 
            query.Language ?? "all", query.Sort, query.Page, query.PageSize);
            
        var queryable = _context.Repositories.Where(r => r.UserId == userId);
        
        if (!string.IsNullOrWhiteSpace(query.Language))
        {
            queryable = queryable.Where(r =>
                r.Language.ToLower() == query.Language.ToLower());
        }

        var totalCount = await queryable.CountAsync();

        queryable = query.Sort.ToLower() switch
        {
            "language" => queryable.OrderBy(r => r.Language),
            "createdat" => queryable.OrderBy(r => r.CreatedAt),
            _ => queryable.OrderBy(r => r.Name)
        };

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        _logger.LogDebug("Found {TotalCount} repositories, returning {ItemCount} items", totalCount, items.Count);
        return (items, totalCount);
    }
}