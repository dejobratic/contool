using Contool.Core.Infrastructure.Utils;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI;

public class ProgressReporter : IProgressReporter, IDisposable
{
    private const double MaxProgress = 1.0;

    private ProgressTask? _task;
    private Task? _renderLoopTask;
    private CancellationTokenSource? _cts;

    private int _currentCount = 0;
    private int? _targetTotal;
    private Func<int>? _getTotal;

    public void Start(string operationName, Func<int> getTotal)
    {
        _cts = new CancellationTokenSource();
        _getTotal = getTotal;

        _renderLoopTask = Task.Run(() =>
        {
            ProgressBar.GetInstance().Start(ctx =>
            {
                _task = ctx.AddTask(operationName, maxValue: MaxProgress);
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
            _task.Value = MaxProgress;

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
