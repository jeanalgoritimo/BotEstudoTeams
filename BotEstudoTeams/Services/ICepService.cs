using BotEstudoTeams.Models;

namespace BotEstudoTeams.Services;

public interface ICepService
{
    Task<CepResponse?> ConsultarAsync(
        string cep,
        CancellationToken cancellationToken = default);
}