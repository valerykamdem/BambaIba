using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BambaIba.Infrastructure.Persistence;
public class BambaIbaDbContextFactory : IDesignTimeDbContextFactory<BambaIbaDbContext>
{
    public BambaIbaDbContext CreateDbContext(string[] args)
    {

        // Charger la config depuis appsettings.Development.json
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<BambaIbaDbContext>();
        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
            .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information);

        return new BambaIbaDbContext(optionsBuilder.Options);
    }
}
