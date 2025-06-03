using Contool.Models;

namespace Contool.Services;

internal interface IOutputWriter
{
    DataSource DataSource { get; }
    Task SaveAsync(OutputContent output, CancellationToken cancellationToken);
}
