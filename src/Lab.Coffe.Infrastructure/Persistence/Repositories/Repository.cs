using System.Data;
using Dapper;
using Lab.Coffe.Application.Interfaces;
using Lab.Coffe.Domain.Entities;
using Lab.Coffe.Infrastructure.Persistence.Dapper;

namespace Lab.Coffe.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _tableName;

    public Repository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _tableName = typeof(T).Name + "s"; // Pluralização simples (Coffee -> Coffees)
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<T>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = $"SELECT * FROM {_tableName}";
        return await connection.QueryAsync<T>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // SQL específico para Coffee (pode ser genérico com reflection, mas mantendo simples)
        var sql = $@"INSERT INTO {_tableName} (Id, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt)
                     VALUES (@Id, @Name, @Description, @Price, @Stock, @IsActive, @CreatedAt, @UpdatedAt)";
        
        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
        
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        entity.UpdateTimestamp();
        
        var sql = $@"UPDATE {_tableName}
                     SET Name = @Name, Description = @Description, Price = @Price, 
                         Stock = @Stock, IsActive = @IsActive, UpdatedAt = @UpdatedAt
                     WHERE Id = @Id";
        
        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE Id = @Id";
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return count > 0;
    }
}
