using BotEstudoTeams.Agents;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;
using BotEstudoTeams.Services;
using BotEstudoTeams.Configurations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services
   .AddOptions<ViaCepOptions>()
   .Bind(
       builder.Configuration.GetSection(
           ViaCepOptions.SectionName))
   .Validate(
       options => Uri.TryCreate(
           options.BaseUrl,
           UriKind.Absolute,
           out _),
       "A configuração ViaCep:BaseUrl deve ser uma URL absoluta.")
   .Validate(
       options =>
           options.TimeoutSeconds is > 0 and <= 60,
       "ViaCep:TimeoutSeconds deve estar entre 1 e 60 segundos.")
   .ValidateOnStart();

builder.Services.AddHttpClient<ICepService, CepService>(
    (serviceProvider, client) =>
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<ViaCepOptions>>()
            .Value;

        client.BaseAddress = new Uri(options.BaseUrl);

        client.Timeout = TimeSpan.FromSeconds(
            options.TimeoutSeconds);
    });

builder.AddAgentApplicationOptions();
builder.Services.AddSingleton<
    IBotCommandService,
    BotCommandService>();
builder.AddAgent<BotEstudoAgent>();

builder.Services.AddSingleton<IStorage, MemoryStorage>();

var app = builder.Build();

app.MapPost(
    "/api/messages",
    async (
        HttpRequest request,
        HttpResponse response,
        IAgentHttpAdapter adapter,
        IAgent agent,
        CancellationToken cancellationToken) =>
    {
        await adapter.ProcessAsync(
            request,
            response,
            agent,
            cancellationToken);
    });

app.Run();