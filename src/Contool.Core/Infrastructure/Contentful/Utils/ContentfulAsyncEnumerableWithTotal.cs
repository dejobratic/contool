using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Infrastructure.Contentful.Utils;

public class ContentfulAsyncEnumerableWithTotal<T>(
    Func<int, int, CancellationToken, Task<ContentfulCollection<T>>> getEntriesAsync,
    int pageSize) : IAsyncEnumerableWithTotal<T>
{
    public int Total { get; private set; }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var skip = 0;
        var isFirst = true;

        while (true)
        {
            var page = await getEntriesAsync(skip, pageSize, cancellationToken);

            if (page?.Items.Any() != true)
                break;

            if (isFirst)
            {
                Total = page.Total;
                isFirst = false;
            }

            foreach (var item in page.Items)
                yield return item;

            skip += page.Items.Count();
        }
    }
}