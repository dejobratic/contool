using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Validation;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Core.Tests.Unit.Features.ContentUpload.Validation;

public class ValidateSystemFieldsTests
{
    [Fact]
    public void GivenEntryWithValidId_WhenValidated_ThenNoErrorsOrWarnings()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("valid-id");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var duplicateIds = new ConcurrentHashSet<string>();
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Act
        ValidationRules.ValidateSystemFields(entry, contentType, 0, duplicateIds, errors, warnings);

        // Assert
        Assert.Empty(errors);
        Assert.Empty(warnings);
        Assert.True(duplicateIds.Contains("valid-id"));
    }

    [Fact]
    public void GivenEntryWithoutId_WhenValidated_ThenWarningIsAdded()
    {
        // Arrange
        var entry = EntryBuilder.CreateWithoutId("blogPost");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var duplicateIds = new ConcurrentHashSet<string>();
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Act
        ValidationRules.ValidateSystemFields(entry, contentType, 0, duplicateIds, errors, warnings);

        // Assert
        Assert.Empty(errors);
        Assert.Single(warnings);
        var warning = warnings[0];
        Assert.Equal(0, warning.EntryIndex);
        Assert.Equal("sys.id", warning.FieldId);
        Assert.Equal(ValidationErrorType.RequiredFieldMissing, warning.Type);
        Assert.Contains("Missing 'sys.id'", warning.Message);
    }

    [Fact]
    public void GivenEntryWithDuplicateId_WhenValidated_ThenErrorIsAdded()
    {
        // Arrange
        var entry1 = EntryBuilder.CreateBlogPost("duplicate-id");
        var entry2 = EntryBuilder.CreateBlogPost("duplicate-id");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var duplicateIds = new ConcurrentHashSet<string>();
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Act
        ValidationRules.ValidateSystemFields(entry1, contentType, 0, duplicateIds, errors, warnings);
        ValidationRules.ValidateSystemFields(entry2, contentType, 1, duplicateIds, errors, warnings);

        // Assert
        Assert.Single(errors);
        Assert.Empty(warnings);
        var error = errors[0];
        Assert.Equal(1, error.EntryIndex);
        Assert.Equal("sys.id", error.FieldId);
        Assert.Equal(ValidationErrorType.DuplicateId, error.Type);
        Assert.Contains("Duplicate ID 'duplicate-id'", error.Message);
    }

    [Fact]
    public void GivenEntryWithoutContentType_WhenValidated_ThenWarningIsAdded()
    {
        // Arrange
        var entry = EntryBuilder.CreateWithoutContentType("test-id");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var duplicateIds = new ConcurrentHashSet<string>();
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Act
        ValidationRules.ValidateSystemFields(entry, contentType, 0, duplicateIds, errors, warnings);

        // Assert
        Assert.Empty(errors);
        Assert.Single(warnings);
        var warning = warnings[0];
        Assert.Equal(0, warning.EntryIndex);
        Assert.Equal("sys.contentType", warning.FieldId);
        Assert.Equal(ValidationErrorType.RequiredFieldMissing, warning.Type);
        Assert.Contains("Missing 'sys.contentType'", warning.Message);
        Assert.Contains("blogPost", warning.Message);
    }

    [Fact]
    public void GivenEntryWithMismatchedContentType_WhenValidated_ThenErrorIsAdded()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("test-id", "product");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var duplicateIds = new ConcurrentHashSet<string>();
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Act
        ValidationRules.ValidateSystemFields(entry, contentType, 0, duplicateIds, errors, warnings);

        // Assert
        Assert.Single(errors);
        Assert.Empty(warnings);
        var error = errors[0];
        Assert.Equal(0, error.EntryIndex);
        Assert.Equal("sys.contentType", error.FieldId);
        Assert.Equal(ValidationErrorType.ContentTypeMismatch, error.Type);
        Assert.Contains("Content type mismatch", error.Message);
        Assert.Contains("expected 'blogPost'", error.Message);
        Assert.Contains("got 'product'", error.Message);
    }

    [Fact]
    public void GivenMultipleEntriesWithMixedValidation_WhenValidated_ThenCorrectErrorsAndWarningsAreAdded()
    {
        // Arrange
        var entry1 = EntryBuilder.CreateBlogPost("valid-id", "blogPost");
        var entry2 = EntryBuilder.CreateWithoutId("blogPost");
        var entry3 = EntryBuilder.CreateBlogPost("valid-id", "blogPost"); // Duplicate
        var entry4 = EntryBuilder.CreateBlogPost("another-id", "product"); // Wrong type
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var duplicateIds = new ConcurrentHashSet<string>();
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Act
        ValidationRules.ValidateSystemFields(entry1, contentType, 0, duplicateIds, errors, warnings);
        ValidationRules.ValidateSystemFields(entry2, contentType, 1, duplicateIds, errors, warnings);
        ValidationRules.ValidateSystemFields(entry3, contentType, 2, duplicateIds, errors, warnings);
        ValidationRules.ValidateSystemFields(entry4, contentType, 3, duplicateIds, errors, warnings);

        // Assert
        Assert.Equal(2, errors.Count);
        Assert.Single(warnings);

        var duplicateError = errors.First(e => e.Type == ValidationErrorType.DuplicateId);
        Assert.Equal(2, duplicateError.EntryIndex);

        var mismatchError = errors.First(e => e.Type == ValidationErrorType.ContentTypeMismatch);
        Assert.Equal(3, mismatchError.EntryIndex);

        var missingIdWarning = warnings[0];
        Assert.Equal(1, missingIdWarning.EntryIndex);
        Assert.Equal(ValidationErrorType.RequiredFieldMissing, missingIdWarning.Type);
    }
}

public class ValidateRequiredFieldsTests
{
    [Fact]
    public void GivenEntryWithAllRequiredFields_WhenValidated_ThenNoErrors()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("test-id", "blogPost");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateRequiredFields(entry, contentType, 0, errors);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void GivenEntryWithMissingRequiredField_WhenValidated_ThenErrorIsAdded()
    {
        // Arrange
        var entry = EntryBuilder.CreateWithMissingRequiredFields("test-id", "blogPost");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateRequiredFields(entry, contentType, 0, errors);

        // Assert
        Assert.Equal(2, errors.Count); // title and body are required

        var titleError = errors.First(e => e.FieldId == "title");
        Assert.Equal(0, titleError.EntryIndex);
        Assert.Equal(ValidationErrorType.RequiredFieldMissing, titleError.Type);
        Assert.Contains("Required field 'Title' (title)", titleError.Message);

        var bodyError = errors.First(e => e.FieldId == "body");
        Assert.Equal(0, bodyError.EntryIndex);
        Assert.Equal(ValidationErrorType.RequiredFieldMissing, bodyError.Type);
        Assert.Contains("Required field 'Body' (body)", bodyError.Message);
    }

    [Fact]
    public void GivenEntryWithEmptyRequiredField_WhenValidated_ThenErrorIsAdded()
    {
        // Arrange
        var entry = EntryBuilder.Create()
            .WithId("test-id")
            .WithContentTypeId("blogPost")
            .WithField("title", "")
            .WithField("body", "Valid content")
            .Build();
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateRequiredFields(entry, contentType, 0, errors);

        // Assert
        Assert.Single(errors);
        var error = errors[0];
        Assert.Equal("title", error.FieldId);
        Assert.Equal(ValidationErrorType.RequiredFieldMissing, error.Type);
    }

    [Fact]
    public void GivenContentTypeWithNoRequiredFields_WhenValidated_ThenNoErrors()
    {
        // Arrange
        var entry = EntryBuilder.CreateMinimal("test-id", "minimal");
        var contentType = ContentTypeBuilder.Create()
            .WithId("minimal")
            .WithField("optionalField", "Optional Field", "Symbol", required: false)
            .Build();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateRequiredFields(entry, contentType, 0, errors);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void GivenContentTypeWithNullFields_WhenValidated_ThenNoErrors()
    {
        // Arrange
        var entry = EntryBuilder.CreateMinimal("test-id", "minimal");
        var contentType = ContentTypeBuilder.Create()
            .WithId("minimal")
            .WithFields() // No fields
            .Build();
        contentType.Fields = null;
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateRequiredFields(entry, contentType, 0, errors);

        // Assert
        Assert.Empty(errors);
    }
}

public class ValidateFieldsExistTests
{
    [Fact]
    public void GivenEntryWithValidFields_WhenValidated_ThenNoErrors()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("test-id", "blogPost");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateFieldsExist(entry, contentType, 0, errors);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void GivenEntryWithInvalidFields_WhenValidated_ThenErrorsAreAdded()
    {
        // Arrange
        var entry = EntryBuilder.CreateWithInvalidFields("test-id", "blogPost");
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateFieldsExist(entry, contentType, 0, errors);

        // Assert
        Assert.Equal(2, errors.Count); // invalidField and anotherInvalidField

        var invalidFieldError = errors.First(e => e.FieldId == "invalidField");
        Assert.Equal(0, invalidFieldError.EntryIndex);
        Assert.Equal(ValidationErrorType.InvalidField, invalidFieldError.Type);
        Assert.Contains("Field 'invalidField' does not exist", invalidFieldError.Message);
        Assert.Contains("blogPost", invalidFieldError.Message);

        var anotherInvalidFieldError = errors.First(e => e.FieldId == "anotherInvalidField");
        Assert.Equal(0, anotherInvalidFieldError.EntryIndex);
        Assert.Equal(ValidationErrorType.InvalidField, anotherInvalidFieldError.Type);
    }

    [Fact]
    public void GivenEntryWithMixOfValidAndInvalidFields_WhenValidated_ThenOnlyInvalidFieldErrorsAreAdded()
    {
        // Arrange
        var entry = EntryBuilder.Create()
            .WithId("test-id")
            .WithContentTypeId("blogPost")
            .WithField("title", "Valid Title")
            .WithField("invalidField", "This should cause an error")
            .WithField("body", "Valid Body")
            .WithField("anotherInvalidField", "This should also cause an error")
            .Build();
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateFieldsExist(entry, contentType, 0, errors);

        // Assert
        Assert.Equal(2, errors.Count);
        Assert.All(errors, error => Assert.Equal(ValidationErrorType.InvalidField, error.Type));
        Assert.Contains(errors, e => e.FieldId == "invalidField");
        Assert.Contains(errors, e => e.FieldId == "anotherInvalidField");
    }

    [Fact]
    public void GivenContentTypeWithNullFields_WhenValidated_ThenAllFieldsAreInvalid()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("test-id", "blogPost");
        var contentType = ContentTypeBuilder.Create()
            .WithId("blogPost")
            .WithFields() // No fields
            .Build();
        contentType.Fields = null;
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateFieldsExist(entry, contentType, 0, errors);

        // Assert
        Assert.Equal(3, errors.Count); // title, body, author from the blog post entry
        Assert.All(errors, error => Assert.Equal(ValidationErrorType.InvalidField, error.Type));
    }
}

public class ValidateLocalesTests
{
    [Fact]
    public void GivenEntryWithValidLocales_WhenValidated_ThenNoErrors()
    {
        // Arrange
        var entry = EntryBuilder.Create()
            .WithId("test-id")
            .WithContentTypeId("blogPost")
            .WithLocalizedField("title", "en-US", "English Title")
            .WithLocalizedField("title", "fr-FR", "French Title")
            .WithField("body", "Non-localized body")
            .Build();
        var locales = LocaleBuilder.CreateMultiple();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateLocales(entry, locales, 0, errors);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void GivenEntryWithInvalidLocale_WhenValidated_ThenErrorIsAdded()
    {
        // Arrange
        var entry = EntryBuilder.Create()
            .WithId("test-id")
            .WithContentTypeId("blogPost")
            .WithLocalizedField("title", "invalid-locale", "Title in invalid locale")
            .WithField("body", "Non-localized body")
            .Build();
        var locales = LocaleBuilder.CreateMultiple();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateLocales(entry, locales, 0, errors);

        // Assert
        Assert.Single(errors);
        var error = errors[0];
        Assert.Equal(0, error.EntryIndex);
        Assert.Equal("title.invalid-locale", error.FieldId);
        Assert.Equal(ValidationErrorType.InvalidLocale, error.Type);
        Assert.Contains("Field 'title.invalid-locale' uses invalid locale 'invalid-locale'", error.Message);
        Assert.Contains("en-US, fr-FR, es-ES", error.Message);
    }

    [Fact]
    public void GivenEntryWithMultipleInvalidLocales_WhenValidated_ThenMultipleErrorsAreAdded()
    {
        // Arrange
        var entry = EntryBuilder.Create()
            .WithId("test-id")
            .WithContentTypeId("blogPost")
            .WithLocalizedField("title", "invalid-locale-1", "Title 1")
            .WithLocalizedField("description", "invalid-locale-2", "Description 2")
            .WithLocalizedField("body", "en-US", "Valid English body")
            .Build();
        var locales = LocaleBuilder.CreateMultiple();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateLocales(entry, locales, 0, errors);

        // Assert
        Assert.Equal(2, errors.Count);
        Assert.All(errors, error => Assert.Equal(ValidationErrorType.InvalidLocale, error.Type));
        Assert.Contains(errors, e => e.FieldId == "title.invalid-locale-1");
        Assert.Contains(errors, e => e.FieldId == "description.invalid-locale-2");
    }

    [Fact]
    public void GivenEntryWithNonLocalizedFields_WhenValidated_ThenNoErrors()
    {
        // Arrange
        var entry = EntryBuilder.Create()
            .WithId("test-id")
            .WithContentTypeId("blogPost")
            .WithField("title", "Non-localized title")
            .WithField("body", "Non-localized body")
            .WithField("fieldWithoutDot", "This field name has no dot")
            .Build();
        var locales = LocaleBuilder.CreateMultiple();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateLocales(entry, locales, 0, errors);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void GivenEmptyLocalesList_WhenValidated_ThenAllLocalizedFieldsAreInvalid()
    {
        // Arrange
        var entry = EntryBuilder.Create()
            .WithId("test-id")
            .WithContentTypeId("blogPost")
            .WithLocalizedField("title", "en-US", "English Title")
            .WithLocalizedField("description", "fr-FR", "French Description")
            .Build();
        var locales = new List<Contentful.Core.Models.Management.Locale>();
        var errors = new List<ValidationError>();

        // Act
        ValidationRules.ValidateLocales(entry, locales, 0, errors);

        // Assert
        Assert.Equal(2, errors.Count);
        Assert.All(errors, error => Assert.Equal(ValidationErrorType.InvalidLocale, error.Type));
    }
}