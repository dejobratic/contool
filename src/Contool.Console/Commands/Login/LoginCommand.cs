using Contentful.Core.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Contool.Console.Commands.Login;

public class LoginCommand : AsyncCommand<LoginCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("--management-token <TOKEN>")]
        [Description("Your Contentful Management API (CMA) token. See [italic LightSkyBlue3]https://www.contentful.com/developers/docs/references/authentication/[/]")]
        public string? ContentManagementToken { get; set; }

        [CommandOption("--delivery-token <TOKEN>")]
        [Description("Your Contentful Content Delivery API (CDA) token. See [italic LightSkyBlue3]https://www.contentful.com/developers/docs/references/authentication/[/]")]
        public string? ContentDeliveryToken { get; set; }

        [CommandOption("--preview-token <TOKEN>")]
        [Description("Your Contentful Content Preview API token. See [italic LightSkyBlue3]https://www.contentful.com/developers/docs/references/authentication/[/]")]
        public string? ContentPreviewToken { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
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
