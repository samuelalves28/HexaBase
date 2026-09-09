using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HexaBase.Infrastructure.Adapters.Out.Persistence;

public sealed class HexaBaseDbContextFactory : IDesignTimeDbContextFactory<HexaBaseDbContext>
{
    public HexaBaseDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString()
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<HexaBaseDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new HexaBaseDbContext(optionsBuilder.Options);
    }

    private static string? GetConnectionString()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        foreach (var directory in GetCandidateDirectories())
        {
            var builder = new ConfigurationBuilder().SetBasePath(directory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            if (!string.IsNullOrWhiteSpace(environment))
            {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);
            }

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }
        }

        return null;
    }

    private static IEnumerable<string> GetCandidateDirectories()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (current is not null)
        {
            yield return current.FullName;

            var apiDirectory = Path.Combine(current.FullName, "src", "HexaBase.Api");
            if (Directory.Exists(apiDirectory))
            {
                yield return apiDirectory;
            }

            var srcDirectory = Path.Combine(current.FullName, "src");
            if (Directory.Exists(srcDirectory))
            {
                yield return srcDirectory;
            }

            current = current.Parent;
        }
    }
}
