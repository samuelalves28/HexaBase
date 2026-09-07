namespace HexaBase.Worker.Adapters.In.Messaging.RabbitMq;

public sealed class RabbitMqOptions
{
    public string HostName { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
    public string ConnectionName { get; init; } = "HexaBase.Worker";
    public string ExchangeName { get; init; } = "hexa.base.user";
    public string QueueName { get; init; } = "hexa.base.user.created";
    public string RoutingKey { get; init; } = "user.created";
}