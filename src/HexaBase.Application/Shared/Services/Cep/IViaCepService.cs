namespace HexaBase.Application.Shared.Services.Cep;

public interface IViaCepService
{
    Task<ViaCepAddress?> GetByCepAsync(
        string cep,
        CancellationToken cancellationToken);
}