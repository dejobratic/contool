using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Features.ContentUpload;

public interface IContentUploader
{
    Task UploadAsync(string contentTypeId, IAsyncEnumerableWithTotal<Entry<dynamic>> entries, IContentfulService contentfulService, bool publish, CancellationToken cancellationToken);
}