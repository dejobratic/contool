using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI;

public class ConsoleProgressReporter : IProgressReporter, IDisposable
{

    private const double MaxProgress = 1.0;

    private ProgressTask? _task;
    private Task? _renderLoopTask;
    private CancellationTokenSource? _cts;

    private Operation? _operation;
    private int _currentCount;
    private int? _targetTotal;
    private Func<int>? _getTotal;

    public void Start(Operation operation, Func<int> getTotal)
    {
        _cts = new CancellationTokenSource();
        _operation = operation;
        _getTotal = getTotal;

        _renderLoopTask = Task.Run(() =>
        {
            ProgressBar.GetInstance().Start(ctx =>
            {
                _task = ctx.AddTask(_operation.ActiveName, maxValue: MaxProgress);
                RunRenderLoop(_cts.Token);
            });
        });
    }

    public void Increment()
    {
        if (_task is null || _getTotal is null)
            return;

        _targetTotal ??= _getTotal();

        var updatedCount = Interlocked.Increment(ref _currentCount);
        var progress = (double)updatedCount / _targetTotal.Value;

        _task.Value = Math.Min(progress, MaxProgress);
    }

    public void Complete()
    {
        if (_task is not null)
        {
            _task.Value = MaxProgress;
            _task.Description = _operation!.CompletedName;
        }

        ResetInternalState();

        _cts?.Cancel();
        _renderLoopTask?.Wait();
    }

    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void RunRenderLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested && !_task?.IsFinished == true)
            Thread.Sleep(100); // keep rendering
    }

    private void ResetInternalState()
    {
        _currentCount = 0;
        _targetTotal = null;
    }
}
