using System.Text.Json;
using BotEstudoTeams.Cards;
using BotEstudoTeams.Models;

namespace BotEstudoTeams.Tests;

public class CepAdaptiveCardTests
{
    [Fact]
    public void Criar_DeveRetornarAdaptiveCard()
    {
        var endereco = CriarEndereco();

        var card = CepAdaptiveCard.Criar(endereco);

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

    [Theory]
    [InlineData("01001-000")]
    [InlineData("Praça da Sé")]
    [InlineData("Sé")]
    [InlineData("São Paulo")]
    [InlineData("SP")]
    [InlineData("Sudeste")]
    [InlineData("11")]
    public void Criar_DeveApresentarDadosDoEndereco(
        string valorEsperado)
    {
        var conteudo = ObterConteudo();
        var textos = ExtrairTextos(conteudo);

        Assert.Contains(
            textos,
            texto => texto.Contains(
                valorEsperado,
                StringComparison.Ordinal));
    }

    [Fact]
    public void Criar_DeveFormatarEstadoComSigla()
    {
        var conteudo = ObterConteudo();
        var textos = ExtrairTextos(conteudo);

        Assert.Contains(
            "São Paulo (SP)",
            textos);
    }

    [Fact]
    public void Criar_QuandoInformacaoEstiverVazia_DeveExibirNaoInformado()
    {
        var endereco = new CepResponse
        {
            Cep = "01001-000",
            Localidade = "São Paulo",
            Uf = "SP"
        };

        var card = CepAdaptiveCard.Criar(endereco);

        var conteudo = Assert.IsType<JsonElement>(
            card.Content);

        var textos = ExtrairTextos(conteudo);

        Assert.Contains(
            "Não informado",
            textos);
    }

    [Fact]
    public void Criar_DeveConterBotaoParaAbrirMapa()
    {
        var conteudo = ObterConteudo();

        var acaoMapa = conteudo
            .GetProperty("actions")
            .EnumerateArray()
            .Single(acao =>
                acao.GetProperty("type").GetString()
                == "Action.OpenUrl");

        Assert.Equal(
            "Abrir no mapa",
            acaoMapa.GetProperty("title").GetString());

        var url = acaoMapa
            .GetProperty("url")
            .GetString();

        Assert.NotNull(url);

        Assert.StartsWith(
            "https://www.bing.com/maps?q=",
            url);

        var urlDecodificada =
            Uri.UnescapeDataString(url);

        Assert.Contains(
            "Praça da Sé",
            urlDecodificada);

        Assert.Contains(
            "São Paulo",
            urlDecodificada);
    }

    [Fact]
    public void Criar_DeveConterBotaoParaNovaConsulta()
    {
        var conteudo = ObterConteudo();

        var acao = conteudo
            .GetProperty("actions")
            .EnumerateArray()
            .Single(item =>
                item.GetProperty("type").GetString()
                == "Action.Submit");

        Assert.Equal(
            "Consultar outro CEP",
            acao.GetProperty("title").GetString());

        var dadosTeams = acao
            .GetProperty("data")
            .GetProperty("msteams");

        Assert.Equal(
            "messageBack",
            dadosTeams.GetProperty("type").GetString());

        Assert.Equal(
            "cep",
            dadosTeams.GetProperty("text").GetString());

        Assert.Equal(
            "cep",
            dadosTeams.GetProperty("value").GetString());
    }

    [Fact]
    public void Criar_QuandoEnderecoForNulo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentNullException>(
            () => CepAdaptiveCard.Criar(null!));
    }

    private static JsonElement ObterConteudo()
    {
        var endereco = CriarEndereco();
        var card = CepAdaptiveCard.Criar(endereco);

        return Assert.IsType<JsonElement>(
            card.Content);
    }

    private static CepResponse CriarEndereco()
    {
        return new CepResponse
        {
            Cep = "01001-000",
            Logradouro = "Praça da Sé",
            Bairro = "Sé",
            Localidade = "São Paulo",
            Uf = "SP",
            Estado = "São Paulo",
            Regiao = "Sudeste",
            Ddd = "11"
        };
    }

    private static IEnumerable<string> ExtrairTextos(
        JsonElement elemento)
    {
        switch (elemento.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (
                    var propriedade
                    in elemento.EnumerateObject())
                {
                    foreach (
                        var texto
                        in ExtrairTextos(propriedade.Value))
                    {
                        yield return texto;
                    }
                }

                break;

            case JsonValueKind.Array:
                foreach (
                    var item
                    in elemento.EnumerateArray())
                {
                    foreach (
                        var texto
                        in ExtrairTextos(item))
                    {
                        yield return texto;
                    }
                }

                break;

            case JsonValueKind.String:
                var valor = elemento.GetString();

                if (valor is not null)
                {
                    yield return valor;
                }

                break;
        }
    }
}