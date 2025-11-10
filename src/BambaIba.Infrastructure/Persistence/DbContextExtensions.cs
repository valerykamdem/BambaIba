using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BambaIba.Infrastructure.Persistence;
public static class DbContextExtensions
{
    /// <summary>
    /// Incrémente une propriété numérique d'une entité en base de données.
    /// </summary>
    public static async Task<int> IncrementAsync<TEntity>(
    this DbContext dbContext,
    Expression<Func<TEntity, bool>> predicate,
    string propertyName,
    int incrementBy = 1,
    CancellationToken cancellationToken = default)
    where TEntity : class
    {
        return await dbContext.Set<TEntity>()
            .Where(predicate)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => EF.Property<int>(e, propertyName),
                             e => EF.Property<int>(e, propertyName) + incrementBy),
                cancellationToken);
    }
}
