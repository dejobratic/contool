using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Services;

public interface IOutputWriter
{
    DataSource DataSource { get; }

    Task SaveAsync(string path, IAsyncEnumerable<dynamic> content, CancellationToken cancellationToken);
}
