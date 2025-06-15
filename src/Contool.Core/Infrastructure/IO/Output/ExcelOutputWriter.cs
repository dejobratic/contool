using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Output;

public class ExcelOutputWriter : IOutputWriter
{
    public DataSource DataSource => DataSource.Excel;

    public Task SaveAsync(string path, IAsyncEnumerable<dynamic> content, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
