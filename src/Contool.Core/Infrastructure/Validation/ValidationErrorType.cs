namespace Contool.Core.Infrastructure.Validation;

public enum ValidationErrorType
{
    DuplicateId,
    ContentTypeMismatch,
    RequiredFieldMissing,
    InvalidField,
    InvalidFieldType,
    InvalidFieldValue
}