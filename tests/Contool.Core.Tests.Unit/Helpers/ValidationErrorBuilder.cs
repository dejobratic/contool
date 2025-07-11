using Contool.Core.Infrastructure.Validation;

namespace Contool.Core.Tests.Unit.Helpers;

public class ValidationErrorBuilder
{
    private int _entryIndex = 0;
    private string _fieldId = "testField";
    private string _message = "Test error message";
    private ValidationErrorType _type = ValidationErrorType.RequiredFieldMissing;

    public ValidationErrorBuilder WithEntryIndex(int entryIndex)
    {
        _entryIndex = entryIndex;
        return this;
    }

    public ValidationErrorBuilder WithFieldId(string fieldId)
    {
        _fieldId = fieldId;
        return this;
    }

    public ValidationErrorBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public ValidationErrorBuilder WithType(ValidationErrorType type)
    {
        _type = type;
        return this;
    }

    public ValidationError Build()
    {
        return new ValidationError(_entryIndex, _fieldId, _message, _type);
    }

    public static ValidationErrorBuilder Create() => new();

    public static ValidationError CreateDefault() => new ValidationErrorBuilder().Build();

    public static ValidationError CreateDuplicateId(int entryIndex = 0, string fieldId = "sys.id") =>
        new ValidationErrorBuilder()
            .WithEntryIndex(entryIndex)
            .WithFieldId(fieldId)
            .WithType(ValidationErrorType.DuplicateId)
            .WithMessage($"Duplicate ID '{fieldId}' found")
            .Build();

    public static ValidationError CreateRequiredFieldMissing(int entryIndex = 0, string fieldId = "title") =>
        new ValidationErrorBuilder()
            .WithEntryIndex(entryIndex)
            .WithFieldId(fieldId)
            .WithType(ValidationErrorType.RequiredFieldMissing)
            .WithMessage($"Required field '{fieldId}' is missing or empty")
            .Build();

    public static ValidationError CreateInvalidField(int entryIndex = 0, string fieldId = "unknownField") =>
        new ValidationErrorBuilder()
            .WithEntryIndex(entryIndex)
            .WithFieldId(fieldId)
            .WithType(ValidationErrorType.InvalidField)
            .WithMessage($"Field '{fieldId}' does not exist in content type")
            .Build();
}