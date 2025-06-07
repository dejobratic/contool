using Contool.Contentful.Services;
using Contool.Models;

namespace Contool.Contentful.Models;

internal class ContentUploadRequest
{
    public Content Content { get; init; } = default!;

    public bool Publish { get; init; }

    public IContentfulService ContentfulService { get; init; } = default!;

    public IContentEntryDeserializer Deserializer { get; init; } = default!;
}