using Contool.Console.Infrastructure.Secrets;
using Contool.Console.Infrastructure.UI;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands.Logout;

public class LogoutCommand(
    IRuntimeContext runtimeContext) : CommandBase<LogoutCommand.Settings>(runtimeContext)
{
    public class Settings : CommandSettings { }

    protected override Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        SecretWriter.Clear();

        AnsiConsole.MarkupLine($"[{Styles.Alert.ToMarkup()}]You are logged out.[/]");

        return Task.FromResult(0);
    }
}
