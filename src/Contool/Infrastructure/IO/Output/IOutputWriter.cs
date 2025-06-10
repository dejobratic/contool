using Contool.Infrastructure.IO.Models;

namespace Contool.Infrastructure.IO.Output;

internal interface IOutputWriter
{
    DataSource DataSource { get; }
    Task SaveAsync(OutputContent output, CancellationToken cancellationToken);
}
