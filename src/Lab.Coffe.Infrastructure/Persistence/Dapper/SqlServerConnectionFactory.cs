using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Lab.Coffe.Infrastructure.Persistence.Dapper;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string not found");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
