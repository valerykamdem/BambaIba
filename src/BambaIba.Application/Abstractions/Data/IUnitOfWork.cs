using System.Data;

namespace BambaIba.Application.Abstractions.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
