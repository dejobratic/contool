using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Features.ContentUpload;

public class ContentUploader(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentUploader
{
    private const int DefaultBatchSize = 50;

    public async Task UploadAsync(ContentUploaderInput input, CancellationToken cancellationToken)
    {
        progressReporter.Start(Operation.Upload, getTotal: () => input.Entries.Total);

        await batchProcessor.ProcessAsync(
            source: input.Entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: (batch, ct) => input.ContentfulService.CreateOrUpdateEntriesAsync(batch, input.PublishEntries, ct),
            cancellationToken: cancellationToken);

        progressReporter.Complete();
    }
}
