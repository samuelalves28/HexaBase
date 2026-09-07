using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HexaBase.Application.Shared.Services.Cep;

namespace HexaBase.Infrastructure.Adapters.Out.External.Cep;

public sealed class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;

    public ViaCepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ViaCepAddress?> GetByCepAsync(
        string cep,
        CancellationToken cancellationToken)
    {
        var normalizedCep = NormalizeCep(cep);
        if (normalizedCep is null)
        {
            return null;
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<ViaCepResponseDto>(
                $"{normalizedCep}/json/",
                cancellationToken);

            if (response is null || response.Error)
            {
                return null;
            }

            return new ViaCepAddress(
                response.Cep ?? normalizedCep,
                response.Logradouro,
                response.Complemento,
                response.Bairro,
                response.Localidade,
                response.Uf);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? NormalizeCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            return null;
        }

        var digits = new string(cep.Where(char.IsDigit).ToArray());
        return digits.Length == 8 ? digits : null;
    }

    private sealed class ViaCepResponseDto
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("erro")]
        public bool Error { get; set; }
    }
}