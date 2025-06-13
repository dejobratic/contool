using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Output;

public interface IOutputWriter
{
    DataSource DataSource { get; }
    Task SaveAsync(OutputContent output, CancellationToken cancellationToken);
}
