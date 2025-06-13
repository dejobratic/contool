using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Input;

public interface IInputReader
{
    DataSource DataSource { get; }
    Task<Content> ReadAsync(string path, CancellationToken cancellationToken);
}
