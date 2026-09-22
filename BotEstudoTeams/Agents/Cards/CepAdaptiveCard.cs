using System.Text.Json;
using BotEstudoTeams.Models;
using Microsoft.Agents.Core.Models;

namespace BotEstudoTeams.Cards;

public static class CepAdaptiveCard
{
    public static Attachment Criar(CepResponse endereco)
    {
        ArgumentNullException.ThrowIfNull(endereco);

        var enderecoCompleto = string.Join(
            ", ",
            new[]
            {
                endereco.Logradouro,
                endereco.Bairro,
                endereco.Localidade,
                endereco.Uf,
                endereco.Cep
            }
            .Where(valor => !string.IsNullOrWhiteSpace(valor)));

        var urlMapa =
            $"https://www.bing.com/maps?q=" +
            Uri.EscapeDataString(enderecoCompleto);

        var conteudo = new Dictionary<string, object?>
        {
            ["$schema"] =
                "http://adaptivecards.io/schemas/adaptive-card.json",

            ["type"] = "AdaptiveCard",
            ["version"] = "1.5",

            ["body"] = new object[]
            {
                new Dictionary<string, object?>
                {
                    ["type"] = "TextBlock",
                    ["text"] = "📍 Endereço encontrado",
                    ["size"] = "Large",
                    ["weight"] = "Bolder",
                    ["color"] = "Accent"
                },

                new Dictionary<string, object?>
                {
                    ["type"] = "TextBlock",
                    ["text"] =
                        "Consulta realizada por meio da API ViaCEP.",
                    ["wrap"] = true,
                    ["isSubtle"] = true,
                    ["spacing"] = "Small"
                },

                new Dictionary<string, object?>
                {
                    ["type"] = "Container",
                    ["style"] = "Emphasis",
                    ["spacing"] = "Medium",

                    ["items"] = new object[]
                    {
                        new Dictionary<string, object?>
                        {
                            ["type"] = "FactSet",

                            ["facts"] = new object[]
                            {
                                CriarFato(
                                    "CEP",
                                    endereco.Cep),

                                CriarFato(
                                    "Logradouro",
                                    endereco.Logradouro),

                                CriarFato(
                                    "Bairro",
                                    endereco.Bairro),

                                CriarFato(
                                    "Cidade",
                                    endereco.Localidade),

                                CriarFato(
                                    "Estado",
                                    FormatarEstado(endereco)),

                                CriarFato(
                                    "Região",
                                    endereco.Regiao),

                                CriarFato(
                                    "DDD",
                                    endereco.Ddd)
                            }
                        }
                    }
                },

                new Dictionary<string, object?>
                {
                    ["type"] = "TextBlock",
                    ["text"] =
                        "Confira os dados antes de utilizá-los.",
                    ["wrap"] = true,
                    ["isSubtle"] = true,
                    ["size"] = "Small",
                    ["spacing"] = "Medium"
                }
            },

            ["actions"] = new object[]
            {
                new Dictionary<string, object?>
                {
                    ["type"] = "Action.OpenUrl",
                    ["title"] = "Abrir no mapa",
                    ["url"] = urlMapa
                },

                new Dictionary<string, object?>
                {
                    ["type"] = "Action.Submit",
                    ["title"] = "Consultar outro CEP",

                    ["data"] = new Dictionary<string, object?>
                    {
                        ["msteams"] =
                            new Dictionary<string, object?>
                            {
                                ["type"] = "messageBack",
                                ["displayText"] =
                                    "Consultar outro CEP",

                                ["text"] = "cep",
                                ["value"] = "cep"
                            }
                    }
                }
            }
        };

        return new Attachment
        {
            ContentType =
                "application/vnd.microsoft.card.adaptive",

            Content =
                JsonSerializer.SerializeToElement(conteudo)
        };
    }

    private static Dictionary<string, object?> CriarFato(
        string titulo,
        string? valor)
    {
        return new Dictionary<string, object?>
        {
            ["title"] = $"{titulo}:",
            ["value"] = string.IsNullOrWhiteSpace(valor)
                ? "Não informado"
                : valor
        };
    }

    private static string FormatarEstado(
        CepResponse endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco.Estado))
        {
            return endereco.Uf ?? "Não informado";
        }

        if (string.IsNullOrWhiteSpace(endereco.Uf))
        {
            return endereco.Estado;
        }

        return $"{endereco.Estado} ({endereco.Uf})";
    }
}