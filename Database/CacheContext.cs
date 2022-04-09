using Microsoft.EntityFrameworkCore;
using yrhacks2022_api.Models;

namespace yrhacks2022_api.Database;

public class CacheContext : DbContext
{
    public DbSet<CacheData> FileCache { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source=cache.db");
    }
}