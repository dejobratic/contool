using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Features.ContentUpload;

public interface IContentUploader
{
    Task UploadAsync(ContentUploaderInput input, CancellationToken cancellationToken);
}