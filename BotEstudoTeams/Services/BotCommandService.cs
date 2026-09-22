namespace BotEstudoTeams.Services;

public class BotCommandService : IBotCommandService
{
    public string Processar(string? mensagem)
    {
        var comando = mensagem?
            .Trim()
            .ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(mensagem))
        {
            return "Por favor, digite uma mensagem.";
        }


        return comando switch
        {
            "olá" or "ola" or "oi" =>
                "Olá! Como posso ajudar você?",

            "ajuda" or "/ajuda" =>
     """
    Estes são os comandos disponíveis:

    • olá — inicia uma conversa
    • menu — exibe o menu interativo
    • cep 29100-000 — consulta um endereço pelo CEP
    • data — mostra a data e a hora
    • sobre — apresenta informações sobre o chatbot
    • ajuda — mostra os comandos disponíveis
    """,

            "data" or "data e hora" =>
              $"Agora são {DateTime.Now:HH:mm} do dia {DateTime.Now:dd/MM/yyyy}.",

            "sobre" =>
                """
                BotEstudoTeams

                Chatbot desenvolvido em C# e ASP.NET Core com o Microsoft 365 Agents SDK.

                Desenvolvido por Jean Paiva da Silva.
                """,

            _ =>
                $"Não reconheci o comando \"{mensagem}\". " +
                "Digite ajuda para conhecer os comandos disponíveis."
        };
    }
}