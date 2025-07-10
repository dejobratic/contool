using Contool.Console.Infrastructure.Secrets;
using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.UI.Services;
using Contool.Console.Infrastructure.Utils;
using Contool.Console.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands.Logout;

public class LogoutCommand(
    ICommandInfoDisplayService commandInfoDisplayService,
    IErrorDisplayService errorDisplayService) : CommandBase<LogoutCommand.Settings>(commandInfoDisplayService, errorDisplayService)
{
    public class Settings : CommandSettings { }

    protected override Task<int> ExecuteCommandAsync(CommandContext context, Settings settings)
    {
        SecretWriter.Clear();

        AnsiConsole.MarkupLine($"[{Styles.Alert.ToMarkup()}]You are logged out.[/]");

        return Task.FromResult(CommandResult.Success);
    }
}
