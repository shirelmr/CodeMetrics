using MetricsAPI.DTOs;
using MetricsAPI.Models;

namespace MetricsAPI.Repositories;

public interface IRepositoryRepository
{
    Task<IEnumerable<Repository>> GetAllAsync();
    Task<(IEnumerable<Repository> Items, int TotalCount)> GetFilteredAsync(RepositoryQueryDto query, int userId);
    Task<Repository?> GetByIdAsync(int id, int userId);
    Task<Repository> AddAsync(Repository repo);
    Task<bool> UpdateAsync(Repository repo, int userId);
    Task<bool> DeleteAsync(int id);
    Task<int> CountAsync();

}