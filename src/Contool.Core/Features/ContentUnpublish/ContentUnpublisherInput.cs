using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentUnpublish;

public class ContentUnpublisherInput
{
    public string ContentTypeId { get; init; } = null!;
    
    public IContentfulService ContentfulService { get; init; } = null!;
}