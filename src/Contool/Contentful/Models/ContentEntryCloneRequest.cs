using Contool.Contentful.Services;

namespace Contool.Contentful.Models;

internal class ContentEntryCloneRequest
{
    public string ContentTypeId { get; init; } = default!;

    public IContentfulService SourceContentfulService { get; init; } = default!;

    public IContentfulService TargetContentfulService { get; init; } = default!;

    public bool ShouldPublish { get; init; }
}
