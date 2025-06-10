using Contool.Infrastructure.IO.Models;

namespace Contool.Infrastructure.IO.Input;

internal interface IInputReader
{
    DataSource DataSource { get; }
    Task<Content> ReadAsync(string path, CancellationToken cancellationToken);
}
