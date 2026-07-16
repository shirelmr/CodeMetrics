using MetricsAPI.DTOs;
using MetricsAPI.Models;

namespace MetricsAPI.Repositories;

public interface IRepositoryRepository
{
    Task<IEnumerable<Repository>> GetAllAsync();
    Task<(IEnumerable<Repository> Items, int TotalCount)> GetFilteredAsync(RepositoryQueryDto query, int userId);
    Task<Repository?> GetByIdAsync(int id);
    Task<Repository> AddAsync(Repository repo);
    Task<bool> UpdateAsync(Repository repo);
    Task<bool> DeleteAsync(int id);
    Task<int> CountAsync();

}