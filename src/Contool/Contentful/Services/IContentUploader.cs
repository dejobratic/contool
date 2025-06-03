using Contool.Contentful.Models;
using Contool.Models;

namespace Contool.Contentful.Services;

internal interface IContentUploader
{
    Task UploadAsync(ContentUploadRequest request, CancellationToken cancellationToken);
}