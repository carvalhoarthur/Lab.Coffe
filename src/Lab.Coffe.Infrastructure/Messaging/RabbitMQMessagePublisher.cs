using System.Text;
using System.Text.Json;
using Lab.Coffe.Application.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lab.Coffe.Infrastructure.Messaging;

public class RabbitMQMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMQConfiguration _config;
    private readonly ILogger<RabbitMQMessagePublisher> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RabbitMQMessagePublisher(
        RabbitMQConfiguration config,
        ILogger<RabbitMQMessagePublisher> logger)
    {
        _config = config;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var factory = new ConnectionFactory
        {
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password,
            VirtualHost = _config.VirtualHost
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Declarar exchange
        _channel.ExchangeDeclare(
            exchange: _config.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        _logger.LogInformation("RabbitMQ connection established to {HostName}:{Port}", _config.HostName, _config.Port);
    }

    public Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : class
    {
        return PublishAsync(message, _config.ExchangeName, routingKey, cancellationToken);
    }

    public Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(message, _jsonOptions);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation(
                "Message published to exchange: {Exchange}, routingKey: {RoutingKey}, messageId: {MessageId}",
                exchange, routingKey, properties.MessageId);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to RabbitMQ");
            return Task.FromException(ex);
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
