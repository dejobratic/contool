using Contool.Models;

namespace Contool.Services;

internal interface IInputReader
{
    DataSource DataSource { get; }
    Task<Content> ReadAsync(string path, CancellationToken cancellationToken);
}
