using Contool.Contentful.Services;
using Contool.Infrastructure.IO.Models;

namespace Contool.Features.EntryUpload;

internal class ContentUploadRequest
{
    public Content Content { get; init; } = default!;

    public bool Publish { get; init; }

    public IContentfulService ContentfulService { get; init; } = default!;

    public IContentEntryDeserializer Deserializer { get; init; } = default!;
}