using Contool.Cli.Infrastructure.Utils;
using Contool.Core.Infrastructure.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Contool.Cli;

public static class Dependencies
{
    public static IServiceCollection AddCliDependencies(this IServiceCollection services)
    {
        return services
            .AddSingleton<IProgressReporter, CliProgressReporter>();
    }
}
