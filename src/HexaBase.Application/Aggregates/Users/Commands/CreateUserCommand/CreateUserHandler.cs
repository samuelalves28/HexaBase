using System.Security.Cryptography;
using HexaBase.Application.Shared.Messages;
using HexaBase.Application.Shared.Services.Authentication;
using HexaBase.Application.Shared.Services.Messaging;
using HexaBase.Domain.Aggregates.User;
using HexaBase.Domain.Aggregates.User.Repository;
using MediatR;

namespace HexaBase.Application.Aggregates.Users.Commands.CreateUserCommand;

public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserCreatedPublisher _userCreatedPublisher;

    public CreateUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUserCreatedPublisher userCreatedPublisher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userCreatedPublisher = userCreatedPublisher;
    }

    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        Validate(request);

        var temporaryPassword = GenerateTemporaryPassword();
        var user = new User(
            request.Name.Trim(),
            request.Email.Trim().ToLowerInvariant(),
            _passwordHasher.Hash(temporaryPassword));

        await _userRepository.CreateAsync(
            user,
            cancellationToken);

        await _userCreatedPublisher.PublishAsync(
            new UserCreatedMessage(
                user.PublicId,
                user.Name,
                user.Email,
                temporaryPassword),
            cancellationToken);

        return user.PublicId;
    }

    private static string GenerateTemporaryPassword()
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%&*";
        Span<byte> buffer = stackalloc byte[16];
        RandomNumberGenerator.Fill(buffer);

        var chars = new char[12];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = alphabet[buffer[i] % alphabet.Length];
        }

        return new string(chars);
    }

    private static void Validate(CreateUserCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request.Name));
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.", nameof(request.Email));
        }

        var email = request.Email.Trim();
        if (!email.Contains('@') || email.StartsWith('@') || email.EndsWith('@'))
        {
            throw new ArgumentException("Email is invalid.", nameof(request.Email));
        }
    }
}
