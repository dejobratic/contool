using Spectre.Console;

namespace Contool.Console.Infrastructure.UI.Services;

public class ConsoleErrorDisplayService : IErrorDisplayService
{
    public void DisplayError(Exception exception)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[{Styles.Alert.ToMarkup()}]ERROR:[/] {exception.Message.EscapeMarkup()}");
    }
}