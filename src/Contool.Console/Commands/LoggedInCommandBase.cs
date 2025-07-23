using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.UI.Services;
using Contool.Console.Infrastructure.Utils.Models;
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
        if (await IsLoginMissingAsync(settings))
        {
            ShowLoginRequiredMessage();
            return CommandResult.Success;
        }

        UpdateRuntimeContext(settings);

        if (ShouldRequireConfirmation(settings))
        {
            if (runtimeContext.IsDryRun)
            {
                ShowDryRunMessage();
            }
            else if (!TryConfirmExecution(context))
            {
                ShowExecutionCancelledMessage();
                return CommandResult.Success;
            }
        }

        return await ExecuteLoggedInCommandAsync(context, settings);
    }

    private async Task<bool> IsLoginMissingAsync(TSettings settings)
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

    private static bool ShouldRequireConfirmation(TSettings settings)
        => settings is WriteSettingsBase;

    private static void ShowLoginRequiredMessage()
    {
        AnsiConsole.MarkupLine(
            $"[{Styles.Alert.ToMarkup()}]Please log in before running this command. " +
            $"Run '[{Styles.Highlight.ToMarkup()}]contool login[/]' and follow the instructions.[/]");
    }

    private static void ShowDryRunMessage()
    {
        AnsiConsole.MarkupLine(
            $"[{Styles.Alert.ToMarkup()}]DRY RUN MODE[/] - " +
            $"[{Styles.Normal.ToMarkup()}] Use [{Styles.Highlight.ToMarkup()}]--apply|-a[/] to execute operations.[/]");
    }

    private static void ShowExecutionCancelledMessage()
        => AnsiConsole.MarkupLine($"[{Styles.Alert.ToMarkup()}]Command execution cancelled.[/]");

    private static bool TryConfirmExecution(CommandContext context)
    {
        var commandName = GetCommandName(context);
        var expectedCode = GenerateRandomTwoDigitCode();

        AnsiConsole.MarkupLine(
            $"[{Styles.Alert.ToMarkup()}]SECURITY CONFIRMATION[/] - " +
            $"[{Styles.Normal.ToMarkup()}]About to execute: [{Styles.Highlight.ToMarkup()}]{commandName}[/][/]");

        var prompt = new TextPrompt<string>(
                $"[{Styles.Normal.ToMarkup()}]Enter [{Styles.Highlight.ToMarkup()}]{expectedCode}[/] to continue:[/]")
            .PromptStyle(Styles.Highlight);

        var userInput = AnsiConsole.Prompt(prompt);
        return userInput == expectedCode;
    }

    private static string GetCommandName(CommandContext context)
        => string.Join(' ', context.Arguments.TakeWhile(arg => !arg.StartsWith('-')));

    private static string GenerateRandomTwoDigitCode()
        => Random.Shared.Next(10, 100).ToString();

    protected abstract Task<int> ExecuteLoggedInCommandAsync(CommandContext context, TSettings settings);
}