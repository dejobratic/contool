using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Infrastructure.IO.Services;

public interface IInputReader
{
    DataSource DataSource { get; }

    IAsyncEnumerableWithTotal<dynamic> ReadAsync(string path, CancellationToken cancellationToken);
}
