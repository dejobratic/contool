using Contool.Core.Infrastructure.Utils;
using Spectre.Console;

namespace Contool.Cli.Infrastructure.Utils;

public class CliProgressReporter : IProgressReporter
{
    public void Report(int current, int total)
    {
        if (total <= 0)
        {
            AnsiConsole.MarkupLine("[red]Progress error: total must be > 0[/]");
            return;
        }

        var percent = (double)current / total;
        var percentStr = percent.ToString("P2");

        if (current < total)
        {
            AnsiConsole.Markup($"\r[yellow]Progress: {current}/{total} ({percentStr})[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"\r[green]✅ Completed: {current}/{total} ({percentStr})[/]");
        }
    }
}
