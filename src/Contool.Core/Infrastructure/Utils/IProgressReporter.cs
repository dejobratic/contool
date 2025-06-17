namespace Contool.Core.Infrastructure.Utils;

public interface IProgressReporter
{
    void Start(string operationName);

    void Report(int current, int total);

    void Complete();
}
