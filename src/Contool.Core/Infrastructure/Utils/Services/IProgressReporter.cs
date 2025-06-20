namespace Contool.Core.Infrastructure.Utils.Services;

public interface IProgressReporter
{
    void Start(string operationName, Func<int> getTotal);

    void Increment();

    void Complete();
}
