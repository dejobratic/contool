using Contool.Core.Contentful.Services;

namespace Contool.Core.Features.EntryDownload;

public class ContentDownloadRequest
{
    public string ContentTypeId { get; init; } = default!;

    public string OutputPath { get; init; } = default!;

    public string OutputFormat { get; init; } = default!;

    public IContentfulService ContentfulService { get; init; } = default!;

    public IContentEntrySerializer Serializer { get; init; } = default!;
}