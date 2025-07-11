using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentfulClient
{
    public bool GetEntriesAsyncWasCalled { get; private set; }
    public string? LastContentTypeId { get; private set; }
    public int? LastPageSize { get; private set; }
    public PagingMode LastPagingMode { get; private set; }
    public CancellationToken LastCancellationToken { get; private set; }

    private IEnumerable<Entry<dynamic>>? _entries;

    public void SetupEntries(IEnumerable<Entry<dynamic>> entries)
    {
        _entries = entries;
    }

    public IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(
        string contentTypeId, 
        int? pageSize = null, 
        PagingMode pagingMode = PagingMode.SkipForward, 
        CancellationToken cancellationToken = default)
    {
        GetEntriesAsyncWasCalled = true;
        LastContentTypeId = contentTypeId;
        LastPageSize = pageSize;
        LastPagingMode = pagingMode;
        LastCancellationToken = cancellationToken;

        var entriesArray = _entries?.ToArray() ?? Array.Empty<Entry<dynamic>>();
        return new AsyncEnumerableWithTotal<Entry<dynamic>>(
            source: AsyncEnumerableFactory.From(entriesArray),
            getTotal: () => entriesArray.Length);
    }
}