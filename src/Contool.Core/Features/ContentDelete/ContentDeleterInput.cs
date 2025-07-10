using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentDelete;

public class ContentDeleterInput
{
    public string ContentTypeId { get; init; } = null!;

    public IContentfulService ContentfulService { get; init; } = null!;

    public bool IncludeArchived { get; init; }
}