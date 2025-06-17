using Contool.Core.Infrastructure.Utils;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI;

public class ProgressReporter : IProgressReporter, IDisposable
{
    private const double MaxValue = 1.0; // Represents 100% completion in the progress bar

    private ProgressTask? _progressTask;
    private Task? _runnerTask;
    private CancellationTokenSource? _cts;

    public void Start(string operationName)
    {
        _cts = new CancellationTokenSource();

        _runnerTask = Task.Run(() =>
        {
            ProgressBar
                .GetInstance()
                .Start(ctx =>
                {
                    _progressTask = ctx.AddTask(operationName, maxValue: MaxValue);
                    while (!_cts.IsCancellationRequested && !_progressTask.IsFinished)
                    {
                        Thread.Sleep(100); // keep rendering
                    }
                });
        });
    }

    public void Report(int current, int total)
    {
        if (_progressTask is null)
            return;

        _progressTask.Value = (double)current / total;
    }

    public void Complete()
    {
        if (_progressTask is not null)
            _progressTask.Value = MaxValue;

        _cts?.Cancel();
        _runnerTask?.Wait();
    }

    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}