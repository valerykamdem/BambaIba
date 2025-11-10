using System.Data;
using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Interfaces;
using Npgsql;

namespace BambaIba.Infrastructure.Persistence;

internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public IDbConnection GetOpenConnection()
    {
        NpgsqlConnection connection = dataSource.OpenConnection();

        return connection;
    }
}

//internal sealed class DbConnectionFactory : IDbConnectionFactory
//{
//    private readonly string _connectionString;

//    public DbConnectionFactory(string connectionString)
//    {
//        _connectionString = connectionString;
//    }

//    public IDbConnection CreateConnection()
//    {
//        var connection = new NpgsqlConnection(_connectionString);
//        connection.Open();

//        return connection;
//    }

//    public IDbConnection GetOpenConnection()
//    {
//        NpgsqlConnection connection = dataSource.OpenConnection();

//        return connection;
//    }
//}
