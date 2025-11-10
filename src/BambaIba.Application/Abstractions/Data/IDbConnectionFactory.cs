using System.Data;

namespace BambaIba.Application.Abstractions.Data;

public interface IDbConnectionFactory
{
    IDbConnection GetOpenConnection();
}
