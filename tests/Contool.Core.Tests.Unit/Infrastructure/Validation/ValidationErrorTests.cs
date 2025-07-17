using Contool.Core.Infrastructure.Validation;

namespace Contool.Core.Tests.Unit.Infrastructure.Validation;

public class ValidationErrorTests
{
    [Fact]
    public void GivenValidParameters_WhenCreatingValidationError_ThenPropertiesAreSetCorrectly()
    {
        // Arrange
        const int entryIndex = 5;
        const string fieldId = "testField";
        const string message = "Test error message";
        const ValidationErrorType type = ValidationErrorType.RequiredFieldMissing;

        // Act
        var error = new ValidationError(entryIndex, fieldId, message, type);

        // Assert
        Assert.Equal(entryIndex, error.EntryIndex);
        Assert.Equal(fieldId, error.FieldId);
        Assert.Equal(message, error.Message);
        Assert.Equal(type, error.Type);
    }

    [Theory]
    [InlineData(0, "id", "Missing ID", ValidationErrorType.RequiredFieldMissing)]
    [InlineData(10, "title", "Invalid title format", ValidationErrorType.InvalidFieldValue)]
    [InlineData(-1, "description", "Description too long", ValidationErrorType.InvalidFieldType)]
    [InlineData(999, "category", "Unknown category", ValidationErrorType.InvalidField)]
    public void GivenVariousParameters_WhenCreatingValidationError_ThenAllPropertiesAreCorrect(
        int entryIndex, string fieldId, string message, ValidationErrorType type)
    {
        // Act
        var error = new ValidationError(entryIndex, fieldId, message, type);

        // Assert
        Assert.Equal(entryIndex, error.EntryIndex);
        Assert.Equal(fieldId, error.FieldId);
        Assert.Equal(message, error.Message);
        Assert.Equal(type, error.Type);
    }

    [Fact]
    public void GivenEmptyStrings_WhenCreatingValidationError_ThenEmptyStringsAreAccepted()
    {
        // Arrange
        const int entryIndex = 0;
        const string fieldId = "";
        const string message = "";
        const ValidationErrorType type = ValidationErrorType.DuplicateId;

        // Act
        var error = new ValidationError(entryIndex, fieldId, message, type);

        // Assert
        Assert.Equal(entryIndex, error.EntryIndex);
        Assert.Equal(fieldId, error.FieldId);
        Assert.Equal(message, error.Message);
        Assert.Equal(type, error.Type);
    }

    [Fact]
    public void GivenValidationError_WhenAccessingProperties_ThenPropertiesAreReadOnly()
    {
        // Arrange & Act
        var error = new ValidationError(1, "test", "message", ValidationErrorType.ContentTypeMismatch);

        // Assert
        Assert.Equal(1, error.EntryIndex);
        Assert.Equal("test", error.FieldId);
        Assert.Equal("message", error.Message);
        Assert.Equal(ValidationErrorType.ContentTypeMismatch, error.Type);
    }

    [Theory]
    [InlineData(ValidationErrorType.DuplicateId)]
    [InlineData(ValidationErrorType.ContentTypeMismatch)]
    [InlineData(ValidationErrorType.RequiredFieldMissing)]
    [InlineData(ValidationErrorType.InvalidField)]
    [InlineData(ValidationErrorType.InvalidFieldType)]
    [InlineData(ValidationErrorType.InvalidFieldValue)]
    [InlineData(ValidationErrorType.InvalidLocale)]
    public void GivenAllValidationErrorTypes_WhenCreatingValidationError_ThenTypeIsSetCorrectly(ValidationErrorType type)
    {
        // Act
        var error = new ValidationError(0, "field", "message", type);

        // Assert
        Assert.Equal(type, error.Type);
    }

    [Fact]
    public void GivenMultipleValidationErrors_WhenComparing_ThenEachHasDistinctProperties()
    {
        // Arrange
        var error1 = new ValidationError(1, "field1", "message1", ValidationErrorType.RequiredFieldMissing);
        var error2 = new ValidationError(2, "field2", "message2", ValidationErrorType.InvalidField);

        // Assert
        Assert.NotEqual(error1.EntryIndex, error2.EntryIndex);
        Assert.NotEqual(error1.FieldId, error2.FieldId);
        Assert.NotEqual(error1.Message, error2.Message);
        Assert.NotEqual(error1.Type, error2.Type);
    }
}