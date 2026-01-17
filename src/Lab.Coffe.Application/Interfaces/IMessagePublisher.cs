namespace Lab.Coffe.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default) where T : class;
}
