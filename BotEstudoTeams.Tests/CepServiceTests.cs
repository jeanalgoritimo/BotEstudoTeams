using System.Net;
using System.Text;
using BotEstudoTeams.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace BotEstudoTeams.Tests;

public class CepServiceTests
{
    [Fact]
    public async Task ConsultarAsync_QuandoCepForValido_DeveRetornarEndereco()
    {
        const string json =
            """
            {
              "cep": "01001-000",
              "logradouro": "Praça da Sé",
              "bairro": "Sé",
              "localidade": "São Paulo",
              "uf": "SP",
              "estado": "São Paulo",
              "ddd": "11"
            }
            """;

        var manipulador = new StubHttpMessageHandler(
            request =>
            {
                Assert.Equal(
                    "https://viacep.com.br/ws/01001000/json/",
                    request.RequestUri?.ToString());

                return CriarResposta(json);
            });

        var service = CriarService(manipulador);

        var resultado = await service.ConsultarAsync("01001-000");

        Assert.NotNull(resultado);
        Assert.Equal("01001-000", resultado.Cep);
        Assert.Equal("Praça da Sé", resultado.Logradouro);
        Assert.Equal("Sé", resultado.Bairro);
        Assert.Equal("São Paulo", resultado.Localidade);
        Assert.Equal("SP", resultado.Uf);
        Assert.Equal("11", resultado.Ddd);
        Assert.Equal(1, manipulador.QuantidadeChamadas);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abcdefgh")]
    [InlineData("12345-6789")]
    public async Task ConsultarAsync_QuandoCepForInvalido_NaoDeveChamarApi(
        string cep)
    {
        var manipulador = new StubHttpMessageHandler(
            _ => throw new InvalidOperationException(
                "A API não deveria ser chamada."));

        var service = CriarService(manipulador);

        var resultado = await service.ConsultarAsync(cep);

        Assert.Null(resultado);
        Assert.Equal(0, manipulador.QuantidadeChamadas);
    }

    [Theory]
    [InlineData("""{ "erro": true }""")]
    [InlineData("""{ "erro": "true" }""")]
    public async Task ConsultarAsync_QuandoCepNaoExistir_DeveRetornarNulo(
        string json)
    {
        var manipulador = new StubHttpMessageHandler(
            _ => CriarResposta(json));

        var service = CriarService(manipulador);

        var resultado = await service.ConsultarAsync("99999-999");

        Assert.Null(resultado);
        Assert.Equal(1, manipulador.QuantidadeChamadas);
    }

    [Fact]
    public async Task ConsultarAsync_DeveRemoverCaracteresDaMascara()
    {
        const string json =
            """
            {
              "cep": "01001-000",
              "localidade": "São Paulo",
              "uf": "SP"
            }
            """;

        var manipulador = new StubHttpMessageHandler(
            request =>
            {
                Assert.Contains(
                    "/ws/01001000/json/",
                    request.RequestUri?.ToString());

                return CriarResposta(json);
            });

        var service = CriarService(manipulador);

        var resultado = await service.ConsultarAsync("01001-000");

        Assert.NotNull(resultado);
    }

    private static CepService CriarService(
      HttpMessageHandler manipulador)
    {
        var httpClient = new HttpClient(manipulador)
        {
            BaseAddress = new Uri("https://viacep.com.br/")
        };

        return new CepService(
            httpClient,
            NullLogger<CepService>.Instance);
    }

    private static HttpResponseMessage CriarResposta(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json")
        };
    }

    private sealed class StubHttpMessageHandler
        : HttpMessageHandler
    {
        private readonly Func<
            HttpRequestMessage,
            HttpResponseMessage> _responder;

        public int QuantidadeChamadas { get; private set; }

        public StubHttpMessageHandler(
            Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            QuantidadeChamadas++;

            return Task.FromResult(_responder(request));
        }
    }
}