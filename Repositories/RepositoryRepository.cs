using MetricsAPI.Data;
using MetricsAPI.Models;
using MetricsAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MetricsAPI.Repositories;

public class RepositoryRepository : IRepositoryRepository
{
    private readonly AppDbContext _context;

    public RepositoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Repository>> GetAllAsync()
    {
        return await _context.Repositories.ToListAsync();
    }

    public async Task<Repository?> GetByIdAsync(int id)
    {
        return await _context.Repositories.FindAsync(id);
    }

    public async Task<Repository> AddAsync(Repository repo)
    {
        repo.CreatedAt = DateTime.UtcNow;
        _context.Repositories.Add(repo);
        await _context.SaveChangesAsync();
        return repo;
    }

    public async Task<bool> UpdateAsync(Repository repo)
    {
        var existing = await _context.Repositories.FindAsync(repo.Id);
        if (existing is null) return false;

        existing.Name = repo.Name;
        existing.Url = repo.Url;
        existing.Language = repo.Language;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var repo = await _context.Repositories.FindAsync(id);
        if (repo is null) return false;

        _context.Repositories.Remove(repo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _context.Repositories.CountAsync();
    }

    public async Task<(IEnumerable<Repository> Items, int TotalCount)> GetFilteredAsync(RepositoryQueryDto query)
    {
        var queryable = _context.Repositories.AsQueryable();
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

        return (items, totalCount);
    }
}