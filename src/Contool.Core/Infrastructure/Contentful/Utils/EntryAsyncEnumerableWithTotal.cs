using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Utils;

// cannot implement dynamic interface with IAsyncEnumerableWithTotal<Entry<dynamic>>
public class EntryAsyncEnumerableWithTotal<T>(
    Func<string, CancellationToken, Task<ContentfulCollection<Entry<T>>>> getEntriesAsync,
    EntryQueryBuilder query,
    PagingMode pagingMode) : IAsyncEnumerableWithTotal<Entry<T>>
{
    public int Total { get; private set; }

    public async IAsyncEnumerator<Entry<T>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var count = 0;
        var skip = 0;
        var isFirst = true;

        while (true)
        {
            var queryString = query
                .Skip(skip)
                .Build();

            var page = await getEntriesAsync(queryString, cancellationToken);

            if (page?.Items.Any() != true)
                break;

            if (isFirst)
            {
                Total = page.Total;
                isFirst = false;
            }

            foreach (var item in page.Items)
            {
                count++;
                yield return item;
            }

            skip = pagingMode switch
            {
                PagingMode.RestartFromBeginning => 0,
                _ => skip + page.Items.Count(),
            };
        }

        Total = count;
    }
}