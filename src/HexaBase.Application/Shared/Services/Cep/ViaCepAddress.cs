namespace HexaBase.Application.Shared.Services.Cep;

public sealed record ViaCepAddress(
    string Cep,
    string? Street,
    string? Complement,
    string? Neighborhood,
    string? City,
    string? State);