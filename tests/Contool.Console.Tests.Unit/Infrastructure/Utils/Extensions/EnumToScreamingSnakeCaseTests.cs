using Contool.Console.Infrastructure.Utils.Extensions;
using Contool.Core.Infrastructure.Validation;
// ReSharper disable InconsistentNaming

namespace Contool.Console.Tests.Unit.Infrastructure.Utils.Extensions;

public class EnumToScreamingSnakeCaseTests
{
    public enum TestEnum
    {
        SimpleValue,
        ComplexCamelCase,
        AnotherTestValue,
        XMLHttpRequest,
        HTMLParser
    }
    
    [Theory]
    [InlineData(TestEnum.SimpleValue, "SIMPLE_VALUE")]
    [InlineData(TestEnum.ComplexCamelCase, "COMPLEX_CAMEL_CASE")]
    [InlineData(TestEnum.AnotherTestValue, "ANOTHER_TEST_VALUE")]
    [InlineData(TestEnum.XMLHttpRequest, "X_M_L_HTTP_REQUEST")]
    [InlineData(TestEnum.HTMLParser, "H_T_M_L_PARSER")]
    public void GivenCustomEnum_WhenConvertedToScreamingSnakeCase_ThenReturnsExpectedFormat(TestEnum enumValue, string expected)
    {
        // Act
        var result = enumValue.ToScreamingSnakeCase();

        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(ValidationErrorType.RequiredFieldMissing, "REQUIRED_FIELD_MISSING")]
    [InlineData(ValidationErrorType.DuplicateId, "DUPLICATE_ID")]
    [InlineData(ValidationErrorType.ContentTypeMismatch, "CONTENT_TYPE_MISMATCH")]
    [InlineData(ValidationErrorType.InvalidField, "INVALID_FIELD")]
    [InlineData(ValidationErrorType.InvalidFieldType, "INVALID_FIELD_TYPE")]
    [InlineData(ValidationErrorType.InvalidFieldValue, "INVALID_FIELD_VALUE")]
    [InlineData(ValidationErrorType.InvalidLocale, "INVALID_LOCALE")]
    public void GivenValidationErrorTypeEnum_WhenConvertedToScreamingSnakeCase_ThenReturnsExpectedFormat(ValidationErrorType enumValue, string expected)
    {
        // Act
        var result = enumValue.ToScreamingSnakeCase();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GivenNullEnum_WhenConvertedToScreamingSnakeCase_ThenThrowsNullReferenceException()
    {
        // Arrange
        Enum? nullEnum = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => nullEnum!.ToScreamingSnakeCase());
    }
}