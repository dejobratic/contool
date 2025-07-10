using Contool.Console.Infrastructure.Logging;
using Contool.Console.Infrastructure.Secrets;
using Contool.Console.Infrastructure.UI.Services;
using Contool.Core;
using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Contool.Console;

public static class Dependencies
{
    public static IServiceCollection BuildServiceCollection()
    {
        var secretsPath = SecretWriter.GetSecretsFilePath();

        var protector = DataProtectionProvider
            .Create("ContoolSecrets")
            .CreateProtector("TokenProtection");

        var configuration = new ConfigurationBuilder()
            .Add(new EncryptedSecretsConfigurationSource(secretsPath, protector))
            .AddUserSecrets<Program>()
            .Build();

        return new ServiceCollection()
            .AddContoolDependencies(configuration)
            .AddConsoleDependencies()
            .AddSingleton(protector)
            
            // Core decorators
            .Decorate<IContentUploadEntryValidator, ContentUploadEntryValidatorConsoleDecorator>();
    }

    private static IServiceCollection AddConsoleDependencies(this IServiceCollection services)
    {
        return services
            // UI
            .AddSingleton<IProgressReporter, ConsoleProgressReporter>()
            .Decorate<IProgressReporter, ConsoleProgressReporterLoggingDecorator>()
            .AddSingleton<ICommandInfoDisplayService, ConsoleCommandInfoDisplayService>()
            .AddSingleton<IContentfulInfoDisplayService, ConsoleContentfulInfoDisplayService>()
            .AddSingleton<IErrorDisplayService, ConsoleErrorDisplayService>()
            .AddSingleton<IOperationsDisplayService, ConsoleOperationDisplayService>()
            .AddSingleton<IValidationSummaryDisplayService, ConsoleValidationSummaryDisplayService>()

            // Logging
            .AddLogging(builder =>
            {
                builder.ClearProviders(); // no console/file logging
                builder.AddProvider(new ConsoleLoggerProvider());
                builder.SetMinimumLevel(LogLevel.None); // Disable all implicit logs
                builder.AddFilter("Contool", LogLevel.Information); // Allow logging only from your namespace
            });
    }
}
