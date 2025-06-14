namespace Contool.Core.Infrastructure.Utils;

public interface IProgressReporter
{
    void Report(int current, int total);
}
