namespace HexaBase.Application.Shared.Services.Authentication;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(
        string password,
        string passwordHash);
}
