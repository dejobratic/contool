using Contool.Core.Features.ContentUpload.Validation;

namespace Contool.Core.Infrastructure.Validation;

public class EntryValidationResult(bool isValid, List<ValidationError> errors)
{
    public bool IsValid { get; } = isValid;
    public IReadOnlyList<ValidationError> Errors { get; } = errors;
}