using Contool.Core.Features.ContentUpload;
using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Validation;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentUploadEntryValidator : IContentUploadEntryValidator
{
    public bool ValidateAsyncWasCalled { get; private set; }
    public CancellationToken LastCancellationToken { get; private set; }
    public bool ShouldThrowException { get; set; }
    public ContentUploaderInput? LastInput { get; private set; }

    private EntryValidationSummary? _validationResult;

    public void SetupValidationResult(EntryValidationSummary validationResult)
    {
        _validationResult = validationResult;
    }

    public Task<EntryValidationSummary> ValidateAsync(
        ContentUploaderInput input,
        CancellationToken cancellationToken = default)
    {
        ValidateAsyncWasCalled = true;
        LastCancellationToken = cancellationToken;
        LastInput = input;

        if (ShouldThrowException)
        {
            throw new InvalidOperationException("Mock validator exception");
        }

        return Task.FromResult(_validationResult ?? new EntryValidationSummary());
    }
}