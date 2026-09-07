namespace HexaBase.Application.Shared.Messages;

public sealed record UserCreatedMessage(
    Guid PublicId,
    string Name,
    string Email,
    string TemporaryPassword);