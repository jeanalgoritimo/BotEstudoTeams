using System.Text.Json;
using Microsoft.Agents.Core.Models;

namespace BotEstudoTeams.Cards;

public static class MenuAdaptiveCard
{
    public static Attachment Criar()
    {
        const string json =
            """
            {
              "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
              "type": "AdaptiveCard",
              "version": "1.5",
              "body": [
                {
                  "type": "TextBlock",
                  "text": "BotEstudo Teams",
                  "size": "Large",
                  "weight": "Bolder",
                  "color": "Accent"
                },
                {
                  "type": "TextBlock",
                  "text": "Microsoft 365 Agents SDK • C# • .NET 8",
                  "wrap": true,
                  "isSubtle": true,
                  "spacing": "Small"
                },
                {
                  "type": "TextBlock",
                  "text": "Assistente de estudos e demonstração de integração com serviços externos.",
                  "wrap": true,
                  "spacing": "Medium"
                },
                {
                  "type": "TextBlock",
                  "text": "Escolha uma opção abaixo ou digite um comando diretamente:",
                  "wrap": true,
                  "spacing": "Medium"
                },
                {
                  "type": "FactSet",
                  "spacing": "Medium",
                  "facts": [
                    {
                      "title": "Tecnologia:",
                      "value": "C# e .NET 8"
                    },
                    {
                      "title": "Plataforma:",
                      "value": "Microsoft 365 Agents SDK"
                    },
                    {
                      "title": "Integração:",
                      "value": "ViaCEP e Bing Maps"
                    },
                    {
                      "title": "Qualidade:",
                      "value": "Testes automatizados com xUnit"
                    },
                    {
                      "title": "Desenvolvedor:",
                      "value": "Jean Paiva da Silva"
                    }
                  ]
                },
                {
                  "type": "TextBlock",
                  "text": "Projeto de estudo • v1.0.0",
                  "wrap": true,
                  "isSubtle": true,
                  "size": "Small",
                  "spacing": "Medium",
                  "separator": true
                }
              ],
              "actions": [
                {
                  "type": "Action.Submit",
                  "title": "Consultar CEP",
                  "data": {
                    "msteams": {
                      "type": "messageBack",
                      "displayText": "Consultar CEP",
                      "text": "cep",
                      "value": "cep"
                    }
                  }
                },
                {
                  "type": "Action.Submit",
                  "title": "Ajuda",
                  "data": {
                    "msteams": {
                      "type": "messageBack",
                      "displayText": "Ajuda",
                      "text": "ajuda",
                      "value": "ajuda"
                    }
                  }
                },
                {
                  "type": "Action.Submit",
                  "title": "Data e hora",
                  "data": {
                    "msteams": {
                      "type": "messageBack",
                      "displayText": "Data e hora",
                      "text": "data",
                      "value": "data"
                    }
                  }
                },
                {
                  "type": "Action.Submit",
                  "title": "Sobre",
                  "data": {
                    "msteams": {
                      "type": "messageBack",
                      "displayText": "Sobre",
                      "text": "sobre",
                      "value": "sobre"
                    }
                  }
                }
              ]
            }
            """;

        return new Attachment
        {
            ContentType =
                "application/vnd.microsoft.card.adaptive",

            Content =
                JsonSerializer.Deserialize<object>(json)
        };
    }
}