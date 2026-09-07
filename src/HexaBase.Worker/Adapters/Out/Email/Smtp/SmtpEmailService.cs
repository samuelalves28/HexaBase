using System.Net;
using System.Net.Mail;
using HexaBase.Application.Shared.Services.Email;
using Microsoft.Extensions.Options;

namespace HexaBase.Worker.Adapters.Out.Email.Smtp;

public sealed class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;

    public SmtpEmailService(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendUserCreatedAsync(
        string email,
        string name,
        string temporaryPassword,
        CancellationToken cancellationToken)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = "Cadastro realizado com sucesso",
            Body = $"Olá {name},\n\nSeu cadastro foi concluído.\nSenha temporária: {temporaryPassword}\n\nAltere a senha no primeiro acesso.",
            IsBodyHtml = false
        };

        message.To.Add(email);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = string.IsNullOrWhiteSpace(_options.UserName)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(_options.UserName, _options.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}