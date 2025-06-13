using Contool.Core.Contentful.Services;
using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Features.EntryUpload;

public class ContentUploadRequest
{
    public Content Content { get; init; } = default!;

    public bool Publish { get; init; }

    public IContentfulService ContentfulService { get; init; } = default!;

    public IContentEntryDeserializer Deserializer { get; init; } = default!;
}