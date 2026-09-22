using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotEstudoTeams.Models;

public class CepResponse
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

    [JsonPropertyName("estado")]
    public string? Estado { get; set; }

    [JsonPropertyName("regiao")]
    public string? Regiao { get; set; }

    [JsonPropertyName("ibge")]
    public string? Ibge { get; set; }

    [JsonPropertyName("ddd")]
    public string? Ddd { get; set; }

    [JsonPropertyName("erro")]
    public JsonElement? Erro { get; set; }

    [JsonIgnore]
    public bool PossuiErro
    {
        get
        {
            if (!Erro.HasValue)
            {
                return false;
            }

            var valor = Erro.Value;

            if (valor.ValueKind == JsonValueKind.True)
            {
                return true;
            }

            if (valor.ValueKind == JsonValueKind.String)
            {
                return bool.TryParse(
                    valor.GetString(),
                    out var resultado) &&
                    resultado;
            }

            return false;
        }
    }
}