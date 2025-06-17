using Contool.Console.Infrastructure.UI;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Contool.Console.Infrastructure.Logging;

public class ConsoleLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);

        AnsiConsole.MarkupLine($"[{Styles.Alert.Foreground}]{message}[/]");
        AnsiConsole.WriteLine(); // Add a new line for better readability
    }
}