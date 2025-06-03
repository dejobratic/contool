using Contool.Contentful.Models;
using Contool.Models;

namespace Contool.Contentful.Services;

internal class ContentDownloader(IContentfulService contentfulService) : IContentDownloader
{
    public async Task<OutputContent> DownloadAsync(ContentDownloadRequest request, CancellationToken cancellationToken)
    {
        var output = CreateOutput(request);
        output.SetHeadings(request.Serializer.FieldNames);

        await foreach (var entry in contentfulService.GetEntriesAsync(request.ContentTypeId, cancellationToken: cancellationToken))
        {
            var serialized = request.Serializer.Serialize(entry);
            output.AddRow(serialized);
        }

        return output;
    }

    private static OutputContent CreateOutput(ContentDownloadRequest request)
    {
        return new OutputContent(
            path: request.OutputPath,
            name: request.ContentTypeId,
            type: request.OutputFormat);
    }
}
