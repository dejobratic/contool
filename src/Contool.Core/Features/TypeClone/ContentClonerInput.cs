using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.TypeClone;

public class ContentClonerInput
{
    public string ContentTypeId { get; init; } = null!;

    public IContentfulService SourceContentfulService { get; init; } = null!;

    public IContentfulService TargetContentfulService { get; init; } = null!;

    public bool PublishEntries { get; init; }
}