using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using BotEstudoTeams.Services;
using BotEstudoTeams.Cards;

namespace BotEstudoTeams.Agents;

public class BotEstudoAgent : AgentApplication
{
    private readonly IBotCommandService _commandService;
    private readonly ICepService _cepService;
    private readonly ILogger<BotEstudoAgent> _logger;
    public BotEstudoAgent(
      AgentApplicationOptions options,
      IBotCommandService commandService,
      ICepService cepService,
      ILogger<BotEstudoAgent> logger)
      : base(options)
    {
        _commandService = commandService;
        _cepService = cepService;
        _logger = logger;

        OnConversationUpdate(
            ConversationUpdateEvents.MembersAdded,
            EnviarBoasVindasAsync);

        OnActivity(
            ActivityTypes.Message,
            ResponderMensagemAsync,
            rank: RouteRank.Last);
    }
    private async Task EnviarBoasVindasAsync(
      ITurnContext turnContext,
      ITurnState turnState,
      CancellationToken cancellationToken)
    {
        foreach (var membro in turnContext.Activity.MembersAdded)
        {
            if (membro.Id == turnContext.Activity.Recipient.Id)
            {
                continue;
            }

            var mensagemBoasVindas =
       """
    Olá! Eu sou o BotEstudo Teams. 👋

    Sou um assistente desenvolvido em C# e .NET 8 com o Microsoft 365 Agents SDK.

    Posso consultar endereços por CEP, informar data e hora e apresentar informações sobre o projeto.

    Escolha uma opção no menu abaixo ou digite "ajuda" para conhecer todos os comandos disponíveis.
    """;

            await turnContext.SendActivityAsync(
                MessageFactory.Text(mensagemBoasVindas),
                cancellationToken);

            var card = MenuAdaptiveCard.Criar();

            await turnContext.SendActivityAsync(
                MessageFactory.Attachment(card),
                cancellationToken);
        }
    }
    private async Task ResponderMensagemAsync(
      ITurnContext turnContext,
      ITurnState turnState,
      CancellationToken cancellationToken)
    {
        var mensagem = turnContext.Activity.Text?.Trim();

        if (string.Equals(
            mensagem,
            "menu",
            StringComparison.OrdinalIgnoreCase))
        {
            var card = MenuAdaptiveCard.Criar();

            await turnContext.SendActivityAsync(
                MessageFactory.Attachment(card),
                cancellationToken);

            return;
        }

        if (EhComandoCep(mensagem))
        {
            await ResponderConsultaCepAsync(
                mensagem!,
                turnContext,
                cancellationToken);

            return;
        }

        var resposta = _commandService.Processar(mensagem);

        await turnContext.SendActivityAsync(
            MessageFactory.Text(resposta),
            cancellationToken);
    }
    private static bool EhComandoCep(string? mensagem)
    {
        return string.Equals(
                   mensagem,
                   "cep",
                   StringComparison.OrdinalIgnoreCase)
               || mensagem?.StartsWith(
                   "cep ",
                   StringComparison.OrdinalIgnoreCase) == true;
    }

    private async Task ResponderConsultaCepAsync(
     string mensagem,
     ITurnContext turnContext,
     CancellationToken cancellationToken)
    {
        var cepInformado = mensagem.Length > 3
            ? mensagem[3..].Trim()
            : string.Empty;

        if (string.IsNullOrWhiteSpace(cepInformado))
        {
            _logger.LogWarning(
                "Consulta de CEP recebida sem o número do CEP.");

            await turnContext.SendActivityAsync(
                MessageFactory.Text(
                    "Informe o CEP após o comando. " +
                    "Exemplo: cep 29100-000"),
                cancellationToken);

            return;
        }

        _logger.LogInformation(
            "Usuário solicitou uma consulta de CEP.");

        try
        {
            var endereco = await _cepService.ConsultarAsync(
                cepInformado,
                cancellationToken);

            if (endereco is null)
            {
                _logger.LogWarning(
                    "A consulta não retornou um endereço.");

                await turnContext.SendActivityAsync(
                    MessageFactory.Text(
                        $"Não encontrei o CEP \"{cepInformado}\". " +
                        "Verifique se ele possui oito números."),
                    cancellationToken);

                return;
            }

            _logger.LogInformation(
                "Endereço encontrado. Cidade: {Cidade}; UF: {Uf}",
                endereco.Localidade,
                endereco.Uf);

            var card = CepAdaptiveCard.Criar(endereco);

            await turnContext.SendActivityAsync(
                MessageFactory.Attachment(card),
                cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(
                exception,
                "Não foi possível concluir a consulta de CEP.");

            await EnviarErroConsultaCepAsync(
                turnContext,
                cancellationToken);
        }
        catch (TaskCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                exception,
                "A consulta de CEP excedeu o tempo limite.");

            await EnviarErroConsultaCepAsync(
                turnContext,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "A consulta de CEP foi cancelada.");

            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Ocorreu um erro inesperado na consulta de CEP.");

            await EnviarErroConsultaCepAsync(
                turnContext,
                cancellationToken);
        }
    }

    private static async Task EnviarErroConsultaCepAsync(
        ITurnContext turnContext,
        CancellationToken cancellationToken)
    {
        await turnContext.SendActivityAsync(
            MessageFactory.Text(
                "Não foi possível consultar o CEP neste momento. " +
                "Tente novamente mais tarde."),
            cancellationToken);
    }
}