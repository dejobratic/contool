using Contool.Console.Infrastructure.Secrets;
using Contool.Console.Infrastructure.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands.Logout;

public class LogoutCommand : CommandBase<LogoutCommand.Settings>
{
    public class Settings : CommandSettings { }

    protected override Task<int> ExecuteCommandAsync(CommandContext context, Settings settings)
    {
        SecretWriter.Clear();

        AnsiConsole.MarkupLine($"[{Styles.Alert.ToMarkup()}]You are logged out.[/]");

        return Task.FromResult(0);
    }
}
