namespace HexaBase.Worker.Adapters.Out.Email.Smtp;

public sealed class SmtpOptions
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 25;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromAddress { get; init; } = "no-reply@hexa.base";
    public string FromName { get; init; } = "HexaBase";
    public bool EnableSsl { get; init; } = false;
}