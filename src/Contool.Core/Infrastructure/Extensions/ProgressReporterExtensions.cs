using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Infrastructure.Extensions;

public static class ProgressReporterExtensions
{
    public static IProgressReporter WithOperationName(this IProgressReporter reporter, string message)
    {
        reporter.Start(message);
        return reporter;
    }
}
