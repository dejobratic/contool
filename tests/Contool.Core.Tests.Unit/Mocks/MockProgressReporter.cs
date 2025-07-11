using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockProgressReporter : IProgressReporter
{
    public bool StartWasCalled { get; private set; }
    public bool IncrementWasCalled { get; private set; }
    public bool CompleteWasCalled { get; private set; }
    public Operation? LastOperation { get; private set; }
    public Func<int>? LastGetTotal { get; private set; }
    public int IncrementCount { get; private set; }

    public void Start(Operation operation, Func<int> getTotal)
    {
        StartWasCalled = true;
        LastOperation = operation;
        LastGetTotal = getTotal;
    }

    public void Increment()
    {
        IncrementWasCalled = true;
        IncrementCount++;
    }

    public void Complete()
    {
        CompleteWasCalled = true;
    }
}