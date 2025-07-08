using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Features.ContentUpload;

public record ContentUploaderInput
{
    public string ContentTypeId { get; init; } = null!;
    
    public IContentfulService ContentfulService { get; init; } = null!;
    
    public IAsyncEnumerableWithTotal<Entry<dynamic>> Entries { get; init; } = null!;

    public bool UploadOnlyValidEntries { get; init; }
    
    public bool PublishEntries { get; init; }
}