namespace Contool.Core.Features.EntryUpload;

public interface IContentUploader
{
    Task UploadAsync(ContentUploadRequest request, CancellationToken cancellationToken);
}