using BambaIba.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BambaIba.Infrastructure.Persistence;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new BambaIbaDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<BambaIbaDbContext>>());

        if (context.Roles.Any())
            return;

        context.Roles.Add(new Role { Id = Guid.CreateVersion7(), Name = "User" });
        context.Roles.Add(new Role { Id = Guid.CreateVersion7(), Name = "Admin" });

        context.SaveChanges();
    }
}
