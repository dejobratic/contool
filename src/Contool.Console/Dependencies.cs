using Contool.Console.Infrastructure.Logging;
using Contool.Console.Infrastructure.UI;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Contool.Console;

public static class Dependencies
{
    public static IServiceCollection AddConsoleDependencies(this IServiceCollection services)
    {
        return services
            .AddSingleton<IProgressReporter, ConsoleProgressReporter>()
            .AddLogging(builder =>
            {
                builder.ClearProviders();                                   // no console/file logging
                builder.AddProvider(new ConsoleLoggerProvider());
                builder.SetMinimumLevel(LogLevel.None);                     // Disable all implicit logs
                builder.AddFilter("Contool", LogLevel.Information); // Allow logging only from your namespace
            });
    }
}
