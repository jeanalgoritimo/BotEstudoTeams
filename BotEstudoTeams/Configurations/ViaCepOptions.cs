namespace BotEstudoTeams.Configurations;

public sealed class ViaCepOptions
{
    public const string SectionName = "ViaCep";

    public string BaseUrl { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 10;
}