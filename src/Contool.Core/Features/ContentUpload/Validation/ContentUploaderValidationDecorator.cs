using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Features.ContentUpload.Validation;

public class ContentUploaderValidationDecorator(
    IContentUploader inner,
    IContentUploadEntryValidator validator) : IContentUploader
{
    public async Task UploadAsync(ContentUploaderInput input, CancellationToken cancellationToken)
    {
        var validationSummary = await validator.ValidateAsync(
            input, cancellationToken);

        if (!input.UploadOnlyValidEntries && validationSummary.Errors.Count != 0)
            throw new InvalidOperationException("Content validation failed. Upload aborted.");

        var validatedEntries = new AsyncEnumerableWithTotal<Entry<dynamic>>(
            validationSummary.ValidEntries.ToAsyncEnumerable(),
            () => validationSummary.ValidEntries.Count);

        var newInput = input with { Entries = validatedEntries };

        await inner.UploadAsync(newInput, cancellationToken);
    }
}