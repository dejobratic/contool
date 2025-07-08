using Contool.Core.Infrastructure.Validation;

namespace Contool.Core.Features.ContentUpload.Validation;

public interface IContentUploadEntryValidator
{
    Task<EntryValidationSummary> ValidateAsync(ContentUploaderInput input, CancellationToken cancellationToken);
}