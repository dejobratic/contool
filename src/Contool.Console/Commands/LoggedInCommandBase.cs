using Contool.Console.Infrastructure.UI;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands;

public abstract class LoggedInCommandBase<TSettings>(
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder) : CommandBase<TSettings>
    where TSettings : SettingsBase
{
    protected override async Task<int> ExecuteCommandAsync(CommandContext context, TSettings settings)
    {
        if (await IsNotLoggedInAsync(settings))
        {
            AnsiConsole.MarkupLine(
                $"[{Styles.Alert.ToMarkup()}]Please log in before running this command. " +
                $"Run '[{Styles.Highlight.ToMarkup()}]contool login[/]' and follow the instructions.[/]");

            return 0;
        }

        UpdateRuntimeContext(settings);

        return await ExecuteLoggedInCommandAsync(context, settings);
    }

    private async Task<bool> IsNotLoggedInAsync(TSettings settings)
    {
        var contentfulService = contentfulServiceBuilder
            .WithSpaceId(settings.SpaceId)
            .WithEnvironmentId(settings.EnvironmentId)
            .Build();

        return !await contentfulService.CanConnectAsync();
    }

    private void UpdateRuntimeContext(TSettings settings)
    {
        var isDryRun = settings is WriteSettingsBase writeSettings
            && writeSettings.Apply is false;

        runtimeContext.SetDryRun(isDryRun);
    }

    protected abstract Task<int> ExecuteLoggedInCommandAsync(CommandContext context, TSettings settings);
}
