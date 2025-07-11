using Contool.Core.Infrastructure.Validation;

namespace Contool.Core.Tests.Unit.Helpers;

public class ValidationWarningBuilder
{
    private int _entryIndex = 0;
    private string _fieldId = "testField";
    private string _message = "Test warning message";
    private ValidationErrorType _type = ValidationErrorType.RequiredFieldMissing;

    public ValidationWarningBuilder WithEntryIndex(int entryIndex)
    {
        _entryIndex = entryIndex;
        return this;
    }

    public ValidationWarningBuilder WithFieldId(string fieldId)
    {
        _fieldId = fieldId;
        return this;
    }

    public ValidationWarningBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public ValidationWarningBuilder WithType(ValidationErrorType type)
    {
        _type = type;
        return this;
    }

    public ValidationWarning Build()
    {
        return new ValidationWarning(_entryIndex, _fieldId, _message, _type);
    }

    public static ValidationWarningBuilder Create() => new();

    public static ValidationWarning CreateDefault() => new ValidationWarningBuilder().Build();

    public static ValidationWarning CreateMissingId(int entryIndex = 0) =>
        new ValidationWarningBuilder()
            .WithEntryIndex(entryIndex)
            .WithFieldId("sys.id")
            .WithType(ValidationErrorType.RequiredFieldMissing)
            .WithMessage("Missing 'sys.id' - ID will be auto-generated")
            .Build();

    public static ValidationWarning CreateMissingContentType(int entryIndex = 0, string contentTypeId = "blogPost") =>
        new ValidationWarningBuilder()
            .WithEntryIndex(entryIndex)
            .WithFieldId("sys.contentType")
            .WithType(ValidationErrorType.RequiredFieldMissing)
            .WithMessage($"Missing 'sys.contentType' - will be set to '{contentTypeId}'")
            .Build();
}