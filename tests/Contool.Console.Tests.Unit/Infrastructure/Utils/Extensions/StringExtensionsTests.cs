using Contool.Console.Infrastructure.Utils.Extensions;

namespace Contool.Console.Tests.Unit.Infrastructure.Utils.Extensions;

public class StringExtensionsTests
{
    public class ToScreamingSnakeCaseTests
    {
        [Theory]
        [InlineData("RequiredFieldMissing", "REQUIRED_FIELD_MISSING")]
        [InlineData("DuplicateId", "DUPLICATE_ID")]
        [InlineData("ContentTypeMismatch", "CONTENT_TYPE_MISMATCH")]
        [InlineData("InvalidField", "INVALID_FIELD")]
        [InlineData("InvalidFieldType", "INVALID_FIELD_TYPE")]
        [InlineData("InvalidFieldValue", "INVALID_FIELD_VALUE")]
        [InlineData("InvalidLocale", "INVALID_LOCALE")]
        public void GivenValidationErrorType_WhenConvertedToScreamingSnakeCase_ThenReturnsExpectedFormat(string input,
            string expected)
        {
            // Act
            var result = input.ToScreamingSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("camelCase", "CAMEL_CASE")]
        [InlineData("PascalCase", "PASCAL_CASE")]
        [InlineData("XMLHttpRequest", "X_M_L_HTTP_REQUEST")]
        public void GivenMixedCaseStrings_WhenConvertedToScreamingSnakeCase_ThenReturnsExpectedFormat(string input,
            string expected)
        {
            // Act
            var result = input.ToScreamingSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("already_snake_case", "ALREADY_SNAKE_CASE")]
        [InlineData("mixed_snake_CaseInput", "MIXED_SNAKE_CASE_INPUT")]
        [InlineData("kebab-case-input", "KEBAB_CASE_INPUT")]
        [InlineData("space separated input", "SPACE_SEPARATED_INPUT")]
        [InlineData("Mixed-Input_With Spaces", "MIXED_INPUT_WITH_SPACES")]
        [InlineData("ALREADY_SCREAMING_SNAKE_CASE", "A_L_R_E_A_D_Y__S_C_R_E_A_M_I_N_G__S_N_A_K_E__C_A_S_E")]
        public void GivenVariousFormats_WhenConvertedToScreamingSnakeCase_ThenReturnsExpectedFormat(string input,
            string expected)
        {
            // Act
            var result = input.ToScreamingSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GivenNullString_WhenConvertedToScreamingSnakeCase_ThenReturnsNull()
        {
            // Arrange
            string input = null!;

            // Act
            var result = input.ToScreamingSnakeCase();

            // Assert
            Assert.Null(result);
        }

        [Theory]

        [InlineData("SIMPLE", "S_I_M_P_L_E")]
        [InlineData("SINGLE_WORD", "S_I_N_G_L_E__W_O_R_D")]
        public void GivenAlreadyScreamingSnakeCase_WhenConvertedToScreamingSnakeCase_ThenReturnsUnchanged(string input,
            string expected)
        {
            // Act
            var result = input.ToScreamingSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("with123Numbers", "WITH123_NUMBERS")]
        [InlineData("HTML5Parser", "H_T_M_L5_PARSER")]
        [InlineData("API2Response", "A_P_I2_RESPONSE")]
        [InlineData("version1_2_3", "VERSION1_2_3")]
        public void GivenStringsWithNumbers_WhenConvertedToScreamingSnakeCase_ThenReturnsExpectedFormat(string input,
            string expected)
        {
            // Act
            var result = input.ToScreamingSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("specialChars!@#", "SPECIAL_CHARS!@#")]
        [InlineData("under_score", "UNDER_SCORE")]
        [InlineData("with.dots", "WITH.DOTS")]
        [InlineData("with/slashes", "WITH/SLASHES")]
        public void GivenStringsWithSpecialCharacters_WhenConvertedToScreamingSnakeCase_ThenReturnsExpectedFormat(
            string input, string expected)
        {
            // Act
            var result = input.ToScreamingSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}