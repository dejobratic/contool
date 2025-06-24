using Contentful.Core;
using Contentful.Core.Configuration;
using Contool.Console.Infrastructure.Secrets;
using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Contool.Console.Commands.Login;

public class LoginCommand(
    IRuntimeContext runtimeContext,
    IHttpClientFactory httpClientFactory,
    IDataProtector protector,
    IOptions<ContentfulOptions> contentfulOptions) : CommandBase<LoginCommand.Settings>(runtimeContext)
{
    public class Settings : SettingsBase
    {
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine($"You can create personal access tokens using the Contentful web app (See [{Styles.Highlight.ToMarkup()}]https://www.contentful.com/developers/docs/references/authentication/#the-management-api[/]). To create a personal access token:");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[{Styles.Highlight.ToMarkup()}]1.[/] Log in to the Contentful web app.");
        AnsiConsole.MarkupLine($"[{Styles.Highlight.ToMarkup()}]2.[/] Open the space that you want to access using the space selector in the top left.");
        AnsiConsole.MarkupLine($"[{Styles.Highlight.ToMarkup()}]3.[/] Click Settings and select CMA tokens from the drop-down list.");
        AnsiConsole.MarkupLine($"[{Styles.Highlight.ToMarkup()}]4.[/] Click Create personal access token. The Create personal access token window is displayed.");
        AnsiConsole.MarkupLine($"[{Styles.Highlight.ToMarkup()}]5.[/] Enter a custom name for your personal access token and click Generate. Your personal access token is created.");
        AnsiConsole.MarkupLine($"[{Styles.Highlight.ToMarkup()}]6.[/] Copy your personal access token to clipboard.");
        AnsiConsole.WriteLine();

        var secrets = new ContentfulOptions
        {
            ManagementApiKey = settings.ContentManagementToken ?? PromptForManagementToken()
        };

        if (!string.IsNullOrWhiteSpace(secrets.ManagementApiKey))
        {
            var httpClient = httpClientFactory.CreateClient();
            var contentfulManagementClient = new ContentfulManagementClient(httpClient, secrets.ManagementApiKey, string.Empty);

            // Load spaces
            var spaces = await contentfulManagementClient.GetSpaces();

            if (spaces?.Any() != true)
                throw new InvalidOperationException("No spaces found.");

            // Build a lookup of Name => ID
            var spacesSet = spaces
                .Where(s => !string.IsNullOrWhiteSpace(s?.Name))
                .ToDictionary(s => s.Name, s => s.SystemProperties.Id);

            var selectedSpaceName = PromptForDefaultSpace(spacesSet.Keys);
            secrets.SpaceId = spacesSet[selectedSpaceName];
            AnsiConsole.MarkupLine($"[{Styles.Normal.Foreground}]Selected default space:[/] [{Styles.AlertAccent.ToMarkup()}]{selectedSpaceName} ({secrets.SpaceId})[/]");

            // Load environments for the selected space
            var environments = await contentfulManagementClient.GetEnvironments(secrets.SpaceId);

            if (environments?.Any() != true)
                throw new InvalidOperationException("No environments found in the selected space.");

            // Build Name => ID map
            var environmentIds = environments
                .Select(e => e.SystemProperties.Id);

            secrets.Environment = PromptForDefaultEnvironment(environmentIds);
            AnsiConsole.MarkupLine($"[{Styles.Normal.Foreground}]Selected default environment:[/] [{Styles.AlertAccent.ToMarkup()}]{secrets.Environment}[/]");
        }

        secrets.DeliveryApiKey = settings.ContentDeliveryToken ?? PromptForDeliveryToken();

        secrets.PreviewApiKey = settings.ContentPreviewToken ?? PromptForPreviewToken();

        SecretWriter.Save(secrets, protector);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic Orange1]Secrets saved.[/]");
        AnsiConsole.WriteLine();

        return 0;
    }

    private string PromptForManagementToken()
    {
        var managementTokenPrompt = CreateTextPrompt(
            text: $"[{Styles.Normal.Foreground}]Enter your Contentful Management API Token:[/]",
            defaultValue: contentfulOptions.Value.ManagementApiKey);

        return AnsiConsole.Prompt(managementTokenPrompt);
    }

    private string PromptForDeliveryToken()
    {
        var deliveryTokenPrompt = CreateTextPrompt(
            text: $"[{Styles.Normal.Foreground}]Enter your Contentful Delivery API Token:[/]",
            defaultValue: contentfulOptions.Value.DeliveryApiKey);

        return AnsiConsole.Prompt(deliveryTokenPrompt);
    }

    private string PromptForPreviewToken()
    {
        var previewTokenPrompt = CreateTextPrompt(
            text: $"[{Styles.Normal.Foreground}]Enter your Contentful Preview API Token:[/]",
            defaultValue: contentfulOptions.Value.PreviewApiKey);

        return AnsiConsole.Prompt(previewTokenPrompt);
    }

    private static string PromptForDefaultSpace(IEnumerable<string> spaces)
    {
        var spacePrompt = CreateSelectionPrompt(
            title: $"[{Styles.Normal.Foreground}]Select your default space:[/]",
            moreChoicesText: $"[{Styles.Dim.ToMarkup()}](Move up and down to reveal more spaces)[/]",
            choices: spaces);

        return AnsiConsole.Prompt(spacePrompt);
    }

    private static string PromptForDefaultEnvironment(IEnumerable<string> environments)
    {
        var environmentPrompt = CreateSelectionPrompt(
            title: $"[{Styles.Normal.Foreground}]Select your default environment:[/]",
            moreChoicesText: $"[{Styles.Dim.ToMarkup()}](Move up and down to reveal more environments)[/]",
            choices: environments);

        return AnsiConsole.Prompt(environmentPrompt);
    }

    private static TextPrompt<string> CreateTextPrompt(string text, string? defaultValue = null)
    {
        return new TextPrompt<string>(text)
            .PromptStyle(Styles.AlertAccent)
            .DefaultValueStyle(Styles.Dim)
            .Secret()
            .DefaultValue(defaultValue ?? string.Empty);
        //.Validate(ValidateContentfulApiKey);
    }

    private static SelectionPrompt<string> CreateSelectionPrompt(
        string title,
        string moreChoicesText,
        IEnumerable<string> choices,
        int pageSize = 10)
    {
        return new SelectionPrompt<string>()
            .Title(title)
            .PageSize(pageSize)
            .MoreChoicesText(moreChoicesText)
            .HighlightStyle(Styles.Alert)
            .AddChoices(choices);
    }
}
