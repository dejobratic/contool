using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentPublish;

public class ContentPublisherInput
{
    public string ContentTypeId { get; init; } = null!;
    
    public IContentfulService ContentfulService { get; init; } = null!;
}