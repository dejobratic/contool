using Microsoft.Extensions.Logging;

namespace Contool.Console.Infrastructure.Logging;

public class ConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
        => new ConsoleLogger();

    public void Dispose()
        => GC.SuppressFinalize(this);
}

