using Contool.Core.Features.ContentUpload;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentUploader : IContentUploader
{
    public bool UploadAsyncWasCalled { get; private set; }
    public ContentUploaderInput? LastInput { get; private set; }
    public CancellationToken LastCancellationToken { get; private set; }
    public bool ShouldThrowException { get; set; }

    public Task UploadAsync(ContentUploaderInput input, CancellationToken cancellationToken = default)
    {
        UploadAsyncWasCalled = true;
        LastInput = input;
        LastCancellationToken = cancellationToken;

        if (ShouldThrowException)
        {
            throw new InvalidOperationException("Mock uploader exception");
        }

        return Task.CompletedTask;
    }
}