using MetricsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MetricsAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Repository> Repositories { get; set; }
}