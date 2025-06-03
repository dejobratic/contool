using Contool.Contentful.Services;
using Contool.Models;

namespace Contool.Contentful.Models;

internal class ContentUploadRequest
{
    public Content Content { get; init; } = default!;
    public bool ShouldPublishContent { get; set; }
    public IContentEntryDeserializer Deserializer { get; init; } = default!;
}