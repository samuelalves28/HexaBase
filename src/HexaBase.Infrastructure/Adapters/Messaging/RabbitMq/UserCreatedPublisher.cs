using System.Text.Json;
using HexaBase.Application.Shared.Messages;
using HexaBase.Application.Shared.Services.Messaging;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;

namespace HexaBase.Infrastructure.Adapters.Messaging.RabbitMq;

public sealed class UserCreatedPublisher : IUserCreatedPublisher
{
    private readonly IConnection _connection;
    private readonly RabbitMqOptions _options;

    public UserCreatedPublisher(
        IConnection connection,
        IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public Task PublishAsync(
        UserCreatedMessage message,
        CancellationToken cancellationToken)
    {
        using var channel = _connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: _options.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null);

        channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            arguments: null);

        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            basicProperties: properties,
            body: payload);

        return Task.CompletedTask;
    }
}