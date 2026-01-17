using Lab.Coffe.Application.Interfaces;
using Lab.Coffe.Domain.Entities;
using Lab.Coffe.Infrastructure.Messaging;
using Lab.Coffe.Infrastructure.Persistence.Dapper;
using Lab.Coffe.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab.Coffe.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();

        // Repositories
        services.AddScoped<IRepository<Coffee>, Repository<Coffee>>();

        // RabbitMQ
        var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>()
            ?? new RabbitMQConfiguration();
        services.AddSingleton(rabbitMQConfig);
        services.AddSingleton<IMessagePublisher, RabbitMQMessagePublisher>();

        // UnitOfWork (simplificado - pode ser expandido)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

internal class UnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Com Dapper, as operações são executadas diretamente
        // Para transações, seria necessário implementar aqui
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
