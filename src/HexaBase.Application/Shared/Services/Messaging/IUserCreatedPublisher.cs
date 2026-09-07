using HexaBase.Application.Shared.Messages;

namespace HexaBase.Application.Shared.Services.Messaging;

public interface IUserCreatedPublisher
{
    Task PublishAsync(
        UserCreatedMessage message,
        CancellationToken cancellationToken);
}