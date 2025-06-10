namespace Contool.Features.EntryUpload;

internal interface IContentUploader
{
    Task UploadAsync(ContentUploadRequest request, CancellationToken cancellationToken);
}