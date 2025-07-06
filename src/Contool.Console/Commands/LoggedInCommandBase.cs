using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.Utils;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands;

public abstract class LoggedInCommandBase<TSettings>(
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandInfoDisplayService commandInfoDisplayService,
    IErrorDisplayService errorDisplayService) : CommandBase<TSettings>(commandInfoDisplayService, errorDisplayService)
    where TSettings : SettingsBase
{
    protected override async Task<int> ExecuteCommandAsync(CommandContext context, TSettings settings)
    {
        if (await IsNotLoggedInAsync(settings))
        {
            AnsiConsole.MarkupLine(
                $"[{Styles.Alert.ToMarkup()}]Please log in before running this command. " +
                $"Run '[{Styles.Highlight.ToMarkup()}]contool login[/]' and follow the instructions.[/]");

            return CommandResult.Success; // Not an error, just needs login
        }

        UpdateRuntimeContext(settings);

        if (runtimeContext.IsDryRun)
        {
            AnsiConsole.MarkupLine(
                $"[{Styles.Alert.ToMarkup()}]DRY RUN MODE[/] - " +
                $"[{Styles.Normal.ToMarkup()}] Use [{Styles.Highlight.ToMarkup()}]--apply|-a[/] to execute operations.[/]",
                Styles.Normal);
        }

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
        var isDryRun = settings is WriteSettingsBase { Apply: false };
        runtimeContext.SetDryRun(isDryRun);
    }

    protected abstract Task<int> ExecuteLoggedInCommandAsync(CommandContext context, TSettings settings);
}