using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Input;

public class JsonInputReader : IInputReader
{
    public DataSource DataSource => DataSource.Json;

    public Task<Content> ReadAsync(string path, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
