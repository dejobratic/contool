using Contool.Core.Features.ContentUpload.Validation;

namespace Contool.Core.Infrastructure.Validation;

public class ValidationWarning(int entryIndex, string fieldId, string message, ValidationErrorType type)
{
    public int EntryIndex { get; } = entryIndex;
    
    public string FieldId { get; } = fieldId;
    
    public string Message { get; } = message;
    
    public ValidationErrorType Type { get; } = type;
}