using System.Net.Http.Json;
using System.Text.RegularExpressions;
using BotEstudoTeams.Models;

namespace BotEstudoTeams.Services;

public class CepService : ICepService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CepService> _logger;
    public CepService(HttpClient httpClient,
                       ILogger<CepService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<CepResponse?> ConsultarAsync(
     string cep,
     CancellationToken cancellationToken = default)
    {
        var cepLimpo = Regex.Replace(
            cep,
            @"\D",
            string.Empty);

        if (cepLimpo.Length != 8)
        {
            _logger.LogWarning(
                "Consulta recusada porque o CEP possui formato inválido.");

            return null;
        }

        _logger.LogInformation(
            "Iniciando consulta ao serviço ViaCEP.");

        try
        {
            var resposta =
                await _httpClient.GetFromJsonAsync<CepResponse>(
                    $"ws/{cepLimpo}/json/",
                    cancellationToken);

            if (resposta is null)
            {
                _logger.LogWarning(
                    "O ViaCEP retornou uma resposta vazia.");

                return null;
            }

            if (resposta.PossuiErro)
            {
                _logger.LogWarning(
                    "O CEP consultado não foi encontrado.");

                return null;
            }

            _logger.LogInformation(
                "Consulta concluída. Cidade: {Cidade}; UF: {Uf}",
                resposta.Localidade,
                resposta.Uf);

            return resposta;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(
                exception,
                "Erro HTTP durante a comunicação com o ViaCEP.");

            throw;
        }
        catch (TaskCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                exception,
                "A consulta ao ViaCEP excedeu o tempo limite.");

            throw;
        }
    }
}