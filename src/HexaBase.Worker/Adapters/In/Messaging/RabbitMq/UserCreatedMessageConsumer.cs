using System.Text.Json;
using HexaBase.Application.Shared.Messages;
using HexaBase.Application.Shared.Services.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HexaBase.Worker.Adapters.In.Messaging.RabbitMq;

public sealed class UserCreatedMessageConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<UserCreatedMessageConsumer> _logger;

    public UserCreatedMessageConsumer(
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<UserCreatedMessageConsumer> logger)
    {
        _connection = connection;
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

        channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<UserCreatedMessage>(args.Body.ToArray());
                if (message is null)
                {
                    channel.BasicNack(args.DeliveryTag, false, true);
                    return;
                }

                using var scope = _serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                await emailService.SendUserCreatedAsync(
                    message.Email,
                    message.Name,
                    message.TemporaryPassword,
                    stoppingToken);

                channel.BasicAck(args.DeliveryTag, false);
                _logger.LogInformation("E-mail enviado com sucesso para {Email}.", message.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao processar mensagem de criação de usuário.");
                channel.BasicNack(args.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}