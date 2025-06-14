using Contool.Core.Infrastructure.Utils;
using Contool.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Contool;

public static class Dependencies
{
    public static IServiceCollection AddCliDependencies(this IServiceCollection services)
    {
        return services
            .AddSingleton<IProgressReporter, CliProgressReporter>();
    }
}
