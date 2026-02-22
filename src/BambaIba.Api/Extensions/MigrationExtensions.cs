using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using BIDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<BIDbContext>();

        dbContext.Database.Migrate();
    }
}
