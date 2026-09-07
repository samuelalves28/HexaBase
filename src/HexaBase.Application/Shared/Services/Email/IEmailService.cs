namespace HexaBase.Application.Shared.Services.Email;

public interface IEmailService
{
    Task SendUserCreatedAsync(
        string email,
        string name,
        string temporaryPassword,
        CancellationToken cancellationToken);
}