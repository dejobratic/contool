using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Console.Infrastructure.UI;

public class ConsoleProgressReporterLoggingDecorator(
    IProgressReporter inner,
    IOperationTracker operationTracker,
    IOperationsDisplayService operationsDisplayService) : IProgressReporter
{
    private Operation _operation = null!;

    public void Start(Operation operation, Func<int> getTotal)
    {
        _operation = operation;
        inner.Start(_operation, getTotal);
    }

    public void Increment()
        => inner.Increment();

    public void Complete()
    {
        inner.Complete();

        var result = operationTracker.GetResult();
        operationsDisplayService.DisplayOperations(result);
    }
}