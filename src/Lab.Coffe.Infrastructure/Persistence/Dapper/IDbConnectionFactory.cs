using System.Data;

namespace Lab.Coffe.Infrastructure.Persistence.Dapper;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
