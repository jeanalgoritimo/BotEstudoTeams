using System.Text.Json;
using BotEstudoTeams.Cards;

namespace BotEstudoTeams.Tests;

public class MenuAdaptiveCardTests
{
    [Fact]
    public void Criar_DeveRetornarAdaptiveCard()
    {
        var card = MenuAdaptiveCard.Criar();

        Assert.NotNull(card);

        Assert.Equal(
            "application/vnd.microsoft.card.adaptive",
            card.ContentType);

        Assert.NotNull(card.Content);
    }

    [Fact]
    public void Criar_DeveUtilizarVersaoCorreta()
    {
        var conteudo = ObterConteudo();

        Assert.Equal(
            "AdaptiveCard",
            conteudo.GetProperty("type").GetString());

        Assert.Equal(
            "1.5",
            conteudo.GetProperty("version").GetString());
    }

    [Fact]
    public void Criar_DeveMostrarInformacoesDoProjeto()
    {
        var conteudo = ObterConteudo();
        var json = conteudo.ToString();

        Assert.Contains("BotEstudo Teams", json);
        Assert.Contains("C# e .NET 8", json);
        Assert.Contains("Microsoft 365 Agents SDK", json);
        Assert.Contains("Jean Paiva da Silva", json);
    }

    [Theory]
    [InlineData("Ajuda", "ajuda")]
    [InlineData("Data e hora", "data")]
    [InlineData("Sobre", "sobre")]
    public void Criar_DeveConterBotaoComComandoCorreto(
        string tituloEsperado,
        string comandoEsperado)
    {
        var conteudo = ObterConteudo();

        var acoes = conteudo
            .GetProperty("actions")
            .EnumerateArray();

        var acao = acoes.FirstOrDefault(item =>
            item.GetProperty("title").GetString() == tituloEsperado);

        Assert.Equal(
            tituloEsperado,
            acao.GetProperty("title").GetString());

        Assert.Equal(
            "Action.Submit",
            acao.GetProperty("type").GetString());

        var comando = acao
            .GetProperty("data")
            .GetProperty("msteams")
            .GetProperty("text")
            .GetString();

        Assert.Equal(comandoEsperado, comando);
    }

    private static JsonElement ObterConteudo()
    {
        var card = MenuAdaptiveCard.Criar();

        return Assert.IsType<JsonElement>(card.Content);
    }
}