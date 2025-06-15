namespace Contool.Core.Features.ContentUpload;

public interface IContentUploader
{
    Task UploadAsync(ContentUploadRequest request, CancellationToken cancellationToken);
}