using Contentful.Core.Configuration;
using Contool.Console.Infrastructure.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Contool.Console.Commands.Login;

public class LoginCommand(
    IRuntimeContext runtimeContext) : CommandBase<LoginCommand.Settings>(runtimeContext)
{
    public class Settings : SettingsBase
    {
        [Secret]
        [CommandOption("--management-token <TOKEN>")]
        [Description("Your Contentful Management API (CMA) token. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/references/authentication/[/]")]
        public string? ContentManagementToken { get; set; }

        [Secret]
        [CommandOption("--delivery-token <TOKEN>")]
        [Description("Your Contentful Content Delivery API (CDA) token. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/references/authentication/[/]")]
        public string? ContentDeliveryToken { get; set; }

        [Secret]
        [CommandOption("--preview-token <TOKEN>")]
        [Description("Your Contentful Content Preview API token. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/references/authentication/[/]")]
        public string? ContentPreviewToken { get; set; }
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        await Task.CompletedTask;

        //var configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "contool", "user-secrets.json");
        //Directory.CreateDirectory(Path.GetDirectoryName(configFile)!);

        var configuration = new ContentfulOptions
        {
            ManagementApiKey = settings.ContentManagementToken ?? AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]Management Token[/]:").Secret()),
            DeliveryApiKey = settings.ContentDeliveryToken ?? AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]Delivery Token[/]:").Secret()),
            PreviewApiKey = settings.ContentPreviewToken ?? AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]Preview Token[/]:").Secret()),
            SpaceId = settings.SpaceId ?? AnsiConsole.Prompt(new TextPrompt<string>("Enter [blue]Space ID[/]:")),
            Environment = settings.EnvironmentId ?? AnsiConsole.Prompt(new TextPrompt<string>("Enter [blue]Environment ID[/]:"))
        };

        //var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
        //await File.WriteAllTextAsync(configFile, json);

        AnsiConsole.MarkupLine("[green]✔ Login details saved successfully.[/]");

        return 0;
    }
}
