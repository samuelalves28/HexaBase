using HexaBase.Application.Shared.Services.Authentication;
using HexaBase.Application.Shared.Services.Cep;
using HexaBase.Application.Shared.Services.Messaging;
using HexaBase.Domain.Aggregates.User.Repository;
using HexaBase.Infrastructure.Adapters.Out.External.Cep;
using HexaBase.Infrastructure.Adapters.Out.Messaging.RabbitMq;
using HexaBase.Infrastructure.Adapters.Out.Persistence;
using HexaBase.Infrastructure.Adapters.Out.Persistence.Repositories;
using HexaBase.Infrastructure.Adapters.Out.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace HexaBase.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<HexaBaseDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddHttpClient<IViaCepService, ViaCepService>(client =>
        {
            var baseAddress = configuration["ExternalServices:ViaCep:BaseAddress"]
                ?? "https://viacep.com.br/ws/";

            client.BaseAddress = new Uri(baseAddress);
        });

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IConnection>(sp =>
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
        services.AddSingleton<IUserCreatedPublisher, UserCreatedPublisher>();

        return services;
    }
}