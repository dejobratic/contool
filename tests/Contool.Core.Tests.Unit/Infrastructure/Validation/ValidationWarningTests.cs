using Contool.Core.Infrastructure.Validation;

namespace Contool.Core.Tests.Unit.Infrastructure.Validation;

public class ValidationWarningTests
{
    [Fact]
    public void GivenValidParameters_WhenCreatingValidationWarning_ThenPropertiesAreSetCorrectly()
    {
        // Arrange
        const int entryIndex = 3;
        const string fieldId = "optionalField";
        const string message = "Optional field warning";
        const ValidationErrorType type = ValidationErrorType.RequiredFieldMissing;

        // Act
        var warning = new ValidationWarning(entryIndex, fieldId, message, type);

        // Assert
        Assert.Equal(entryIndex, warning.EntryIndex);
        Assert.Equal(fieldId, warning.FieldId);
        Assert.Equal(message, warning.Message);
        Assert.Equal(type, warning.Type);
    }

    [Theory]
    [InlineData(0, "sys.id", "Missing ID - will be auto-generated", ValidationErrorType.RequiredFieldMissing)]
    [InlineData(5, "sys.contentType", "Missing content type", ValidationErrorType.RequiredFieldMissing)]
    [InlineData(10, "deprecated", "Field is deprecated", ValidationErrorType.InvalidField)]
    [InlineData(-1, "locale", "Invalid locale format", ValidationErrorType.InvalidLocale)]
    public void GivenVariousParameters_WhenCreatingValidationWarning_ThenAllPropertiesAreCorrect(
        int entryIndex, string fieldId, string message, ValidationErrorType type)
    {
        // Act
        var warning = new ValidationWarning(entryIndex, fieldId, message, type);

        // Assert
        Assert.Equal(entryIndex, warning.EntryIndex);
        Assert.Equal(fieldId, warning.FieldId);
        Assert.Equal(message, warning.Message);
        Assert.Equal(type, warning.Type);
    }

    [Fact]
    public void GivenEmptyStrings_WhenCreatingValidationWarning_ThenEmptyStringsAreAccepted()
    {
        // Arrange
        const int entryIndex = 0;
        const string fieldId = "";
        const string message = "";
        const ValidationErrorType type = ValidationErrorType.InvalidFieldValue;

        // Act
        var warning = new ValidationWarning(entryIndex, fieldId, message, type);

        // Assert
        Assert.Equal(entryIndex, warning.EntryIndex);
        Assert.Equal(fieldId, warning.FieldId);
        Assert.Equal(message, warning.Message);
        Assert.Equal(type, warning.Type);
    }

    [Fact]
    public void GivenValidationWarning_WhenAccessingProperties_ThenPropertiesAreReadOnly()
    {
        // Arrange
        var warning = new ValidationWarning(2, "test", "warning message", ValidationErrorType.InvalidFieldType);

        // Act & Assert - Properties should not have setters (compile-time check)
        Assert.Equal(2, warning.EntryIndex);
        Assert.Equal("test", warning.FieldId);
        Assert.Equal("warning message", warning.Message);
        Assert.Equal(ValidationErrorType.InvalidFieldType, warning.Type);
    }

    [Theory]
    [InlineData(ValidationErrorType.DuplicateId)]
    [InlineData(ValidationErrorType.ContentTypeMismatch)]
    [InlineData(ValidationErrorType.RequiredFieldMissing)]
    [InlineData(ValidationErrorType.InvalidField)]
    [InlineData(ValidationErrorType.InvalidFieldType)]
    [InlineData(ValidationErrorType.InvalidFieldValue)]
    [InlineData(ValidationErrorType.InvalidLocale)]
    public void GivenAllValidationErrorTypes_WhenCreatingValidationWarning_ThenTypeIsSetCorrectly(ValidationErrorType type)
    {
        // Act
        var warning = new ValidationWarning(0, "field", "message", type);

        // Assert
        Assert.Equal(type, warning.Type);
    }

    [Fact]
    public void GivenValidationErrorAndWarning_WhenComparingStructure_ThenBothHaveSameProperties()
    {
        // Arrange
        const int entryIndex = 1;
        const string fieldId = "testField";
        const string message = "Test message";
        const ValidationErrorType type = ValidationErrorType.RequiredFieldMissing;

        var error = new ValidationError(entryIndex, fieldId, message, type);
        var warning = new ValidationWarning(entryIndex, fieldId, message, type);

        // Assert - Both should have identical property values
        Assert.Equal(error.EntryIndex, warning.EntryIndex);
        Assert.Equal(error.FieldId, warning.FieldId);
        Assert.Equal(error.Message, warning.Message);
        Assert.Equal(error.Type, warning.Type);
    }

    [Fact]
    public void GivenMultipleValidationWarnings_WhenComparing_ThenEachHasDistinctProperties()
    {
        // Arrange
        var warning1 = new ValidationWarning(1, "field1", "message1", ValidationErrorType.RequiredFieldMissing);
        var warning2 = new ValidationWarning(2, "field2", "message2", ValidationErrorType.InvalidLocale);

        // Assert
        Assert.NotEqual(warning1.EntryIndex, warning2.EntryIndex);
        Assert.NotEqual(warning1.FieldId, warning2.FieldId);
        Assert.NotEqual(warning1.Message, warning2.Message);
        Assert.NotEqual(warning1.Type, warning2.Type);
    }
}