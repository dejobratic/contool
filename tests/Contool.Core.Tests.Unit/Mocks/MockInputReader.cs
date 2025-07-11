using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockInputReader : IInputReader
{
    private readonly Dictionary<string, object?>[] _data;

    public MockInputReader(Dictionary<string, object?>[] data)
    {
        _data = data;
        DataSource = new FileDataSource("Mock", ".mock");
    }

    public DataSource DataSource { get; }

    public IAsyncEnumerableWithTotal<dynamic> ReadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return new AsyncEnumerableWithTotal<dynamic>(
            source: AsyncEnumerableFactory.From(_data.Cast<dynamic>()),
            getTotal: () => _data.Length);
    }
}