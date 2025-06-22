using Contool.Console.Infrastructure.UI.Extensions;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI;

public class ConsoleProgressReporterLogingDecorator(
    IProgressReporter inner,
    IEntriesOperationTracker operationTracker) : IProgressReporter
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

        var result = operationTracker.GetResults();
        if(result.Operations.Count == 0)
        {
            //AnsiConsole.WriteLine($"[{Styles.Alert.Foreground}]No entries found for operation '{_operation.Name}'.[/]");
            return;
        }

        var table = result.DrawTable();

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        //AnsiConsole.WriteLine();
    }
}