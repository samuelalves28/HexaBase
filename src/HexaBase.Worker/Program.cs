using HexaBase.Application.Shared.Services.Email;
using HexaBase.Worker.Adapters.In.Messaging.RabbitMq;
using HexaBase.Worker.Adapters.Out.Email.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddSingleton<IConnection>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value;
    var factory = new ConnectionFactory
    {
        HostName = options.HostName,
        Port = options.Port,
        UserName = options.Username,
        Password = options.Password,
        VirtualHost = options.VirtualHost,
        DispatchConsumersAsync = true,
        AutomaticRecoveryEnabled = true
    };

    return factory.CreateConnection(options.ConnectionName);
});

builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
builder.Services.AddHostedService<UserCreatedMessageConsumer>();

var host = builder.Build();
host.Run();