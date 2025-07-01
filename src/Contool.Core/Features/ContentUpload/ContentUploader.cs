using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentUpload;

public class ContentUploader(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentUploader
{
    private const int DefaultBatchSize = 50;

    public async Task UploadAsync(string contentTypeId, IAsyncEnumerableWithTotal<Entry<dynamic>> entries, IContentfulService contentfulService, bool publish, CancellationToken cancellationToken)
    {
        progressReporter.Start(Operation.Upload, getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: (batch, ct) => contentfulService.CreateOrUpdateEntriesAsync(batch, publish, ct),
            cancellationToken: cancellationToken);

        progressReporter.Complete();
    }
}
