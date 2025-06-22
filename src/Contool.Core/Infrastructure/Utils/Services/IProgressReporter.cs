using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Utils.Services;

public interface IProgressReporter
{
    void Start(Operation operation, Func<int> getTotal);

    void Increment();

    void Complete();
}
