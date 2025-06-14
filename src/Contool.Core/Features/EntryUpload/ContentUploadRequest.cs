using Contool.Core.Contentful.Services;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Features.EntryUpload;

public class ContentUploadRequest
{
    public IAsyncEnumerableWithTotal<dynamic> Content { get; init; } = default!;

    public bool Publish { get; init; }

    public IContentfulService ContentfulService { get; init; } = default!;

    public IContentEntryDeserializer Deserializer { get; init; } = default!;
}