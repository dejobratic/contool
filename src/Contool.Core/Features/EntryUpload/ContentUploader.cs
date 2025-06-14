using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils;
using System.Runtime.CompilerServices;

namespace Contool.Core.Features.EntryUpload;

public class ContentUploader : IContentUploader
{
    public async Task UploadAsync(ContentUploadRequest request, CancellationToken cancellationToken)
    {
        var entries = GetEntriesForUploading(
            request, cancellationToken);

        await request.ContentfulService.CreateOrUpdateEntriesAsync(
            entries, request.Publish, cancellationToken);
    }

    private static AsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesForUploading(
        ContentUploadRequest request, CancellationToken cancellationToken)
    {
        return new AsyncEnumerableWithTotal<Entry<dynamic>>(
            GetDeserializedentries(request, cancellationToken),
            getTotal: () => request.Content.Total);
    }

    private static async IAsyncEnumerable<Entry<dynamic>> GetDeserializedentries(
        ContentUploadRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var row in request.Content.WithCancellation(cancellationToken))
            yield return request.Deserializer.Deserialize(row);
    }
}