using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Features.ContentDelete;

public class ContentDeleter(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentDeleter
{
    private const int DefaultBatchSize = 100;

    public async Task DeleteAsync(ContentDeleterInput input, CancellationToken cancellationToken = default)
    {
        var entries = input.ContentfulService.GetEntriesAsync(
            input.ContentTypeId, DefaultBatchSize, PagingMode.RestartFromBeginning, cancellationToken);

        await DeleteEntriesAsync(
            entries, input.ContentfulService, input.IncludeArchived, cancellationToken);
    }

    private async Task DeleteEntriesAsync(
        IAsyncEnumerableWithTotal<Entry<dynamic>> entries,
        IContentfulService contentfulService,
        bool includeArchived,
        CancellationToken cancellationToken)
    {
        progressReporter.Start(Operation.Delete, getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: contentfulService.DeleteEntriesAsync,
            batchItemFilter: entry => includeArchived || !entry.IsArchived(),
            cancellationToken: cancellationToken);

        progressReporter.Complete();
    }
}
