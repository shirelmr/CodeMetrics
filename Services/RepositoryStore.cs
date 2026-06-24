using MetricsAPI.Models;

namespace MetricsAPI.Services;

public class RepositoryStore
{
    private readonly List<Repository> _repositories = new();
    private int _nextId = 1;

    public IReadOnlyList<Repository> GetAll() => _repositories.AsReadOnly();
    public Repository? GetById(int id) =>
    _repositories.FirstOrDefault(r => r.Id == id);

    public Repository Add(Repository repo)
    {
        repo.Id = _nextId++;
        _repositories.Add(repo);
        return repo;
    }

    public bool Update(Repository updatedRepo)
    {
        var existing = GetById(updatedRepo.Id);
        if (existing is null) return false;
        existing.Name = updatedRepo.Name;
        existing.Url = updatedRepo.Url;
        existing.Language = updatedRepo.Language;
        return true;
    }

    public bool Delete(int id)
    {
        var repo = GetById(id);
        if (repo is null) return false;

        _repositories.Remove(repo);
        return true;
    }

    public int Count() => _repositories.Count;
}