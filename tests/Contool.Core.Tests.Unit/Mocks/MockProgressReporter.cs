using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockProgressReporter : IProgressReporter
{
    public int IncrementCount { get; private set; }
    public bool IsComplete { get; private set; }

    public void Start(string operationName, Func<int> getTotal) { }

    public void Increment()
    {
        IncrementCount++;
    }

    public void Complete()
    {
        IsComplete = true;
    }
}