using BotEstudoTeams.Services;

namespace BotEstudoTeams.Tests;

public class BotCommandServiceTests
{
    private readonly BotCommandService _service = new();

    [Theory]
    [InlineData("oi")]
    [InlineData("olá")]
    [InlineData("ola")]
    [InlineData("OI")]
    [InlineData("  oi  ")]
    public void Processar_QuandoReceberSaudacao_DeveResponderCorretamente(
        string mensagem)
    {
        var resposta = _service.Processar(mensagem);

        Assert.Equal(
            "Olá! Como posso ajudar você?",
            resposta);
    }

    [Theory]
    [InlineData("data")]
    [InlineData("Data e Hora")]
    [InlineData("DATA E HORA")]
    [InlineData("  data  ")]
    public void Processar_QuandoSolicitarData_DeveMostrarDataEHora(
        string mensagem)
    {
        var dataAntesDaExecucao = DateTime.Now;

        var resposta = _service.Processar(mensagem);

        var dataDepoisDaExecucao = DateTime.Now;

        Assert.StartsWith("Agora são ", resposta);

        Assert.True(
            resposta.Contains(
                dataAntesDaExecucao.ToString("dd/MM/yyyy")) ||
            resposta.Contains(
                dataDepoisDaExecucao.ToString("dd/MM/yyyy")));

        Assert.Matches(
            @"\d{2}:\d{2}",
            resposta);
    }

    [Theory]
    [InlineData("ajuda")]
    [InlineData("/ajuda")]
    [InlineData("AJUDA")]
    [InlineData("  ajuda  ")]
    public void Processar_QuandoSolicitarAjuda_DeveMostrarComandos(
        string mensagem)
    {
        var resposta = _service.Processar(mensagem);

        Assert.Contains(
            "comandos disponíveis",
            resposta,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "menu",
            resposta,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "data",
            resposta,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "sobre",
            resposta,
            StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("sobre")]
    [InlineData("SOBRE")]
    [InlineData("  sobre  ")]
    public void Processar_QuandoSolicitarSobre_DeveMostrarInformacoes(
        string mensagem)
    {
        var resposta = _service.Processar(mensagem);

        Assert.Contains("BotEstudoTeams", resposta);
        Assert.Contains("Microsoft 365 Agents SDK", resposta);
        Assert.Contains("Jean Paiva da Silva", resposta);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Processar_QuandoMensagemForInvalida_DeveOrientarUsuario(
        string? mensagem)
    {
        var resposta = _service.Processar(mensagem);

        Assert.Equal(
            "Por favor, digite uma mensagem.",
            resposta);
    }

    [Theory]
    [InlineData("qualquer coisa")]
    [InlineData("comando inexistente")]
    [InlineData("xyz")]
    public void Processar_QuandoComandoForDesconhecido_DeveOrientarUsuario(
        string mensagem)
    {
        var resposta = _service.Processar(mensagem);

        Assert.Contains(
            "Não reconheci",
            resposta);

        Assert.Contains(
            mensagem,
            resposta);

        Assert.Contains(
            "ajuda",
            resposta,
            StringComparison.OrdinalIgnoreCase);
    }
}