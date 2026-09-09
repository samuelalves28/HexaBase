using HexaBase.Domain.Aggregates.User;
using HexaBase.Infrastructure.Adapters.Out.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HexaBase.Infrastructure.Adapters.Out.Persistence;

public sealed class HexaBaseDbContext(DbContextOptions<HexaBaseDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}