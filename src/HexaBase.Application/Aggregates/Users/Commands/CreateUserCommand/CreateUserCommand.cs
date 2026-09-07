using MediatR;

namespace HexaBase.Application.Aggregates.Users.Commands.CreateUserCommand;

public sealed record CreateUserCommand(
    string Name,
    string Email) : IRequest<Guid>;
