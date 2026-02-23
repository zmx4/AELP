using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using QuickToAELP.Helper;

namespace QuickToAELP.Pages;

internal sealed partial class SearchPage : ContentPage
{
    private readonly SearchContentPage _contentPage = new();

    public override IContent[] GetContent() => [_contentPage];

    public SearchPage()
    {
        // Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Icon = new("🔍"); // 🔍 图标
        Title = "Search Word";
        Name = "Search";
    }
}
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Sample code")]
internal sealed partial class SearchContentPage : FormContent
{
    public SearchContentPage()
    {
        TemplateJson = 
            $$"""
            {
                "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
                "type": "AdaptiveCard",
                "version": "1.6",
                "body": [
                    {
                        "type": "TextBlock",
                        "text": "Search Word",
                        "weight": "Bolder",
                        "size": "Medium"
                    },
                    {
                        "type": "Input.Text",
                        "id": "searchQuery",
                        "placeholder": "Enter a word to search"
                    }
                ],
                "actions": [
                    {
                        "type": "Action.Submit",
                        "title": "Search"
                    }
                ]
            }
            """;
    }
    public override CommandResult SubmitForm(string payload)
    {
        var formInput = JsonNode.Parse(payload)?.AsObject();
        if (formInput is null)
        {
            return CommandResult.GoHome();
        }

        var searchedWord = formInput["searchQuery"]?.ToString() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(searchedWord))
        {
            var programPath = PathHelper.GetAppFolderPath();
            return new Command.StartAppCommand(programPath, "search " + searchedWord).Invoke();
        }

        return CommandResult.GoHome();
    }
}
