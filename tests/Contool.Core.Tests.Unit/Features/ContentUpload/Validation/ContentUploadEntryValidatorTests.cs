using Contool.Core.Features.ContentUpload;
using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Validation;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentUpload.Validation;

public class ContentUploadEntryValidatorTests
{
    private readonly ContentUploadEntryValidator _validator;
    private readonly MockContentfulService _mockContentfulService;

    public ContentUploadEntryValidatorTests()
    {
        _validator = new ContentUploadEntryValidator();
        _mockContentfulService = new MockContentfulService();
    }

    [Fact]
    public async Task GivenValidEntries_WhenValidated_ThenAllEntriesAreValid()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var validEntries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(validEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.ValidEntries.Count);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public async Task GivenEntriesWithMissingRequiredFields_WhenValidated_ThenErrorsAreReported()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var invalidEntries = new[]
        {
            EntryBuilder.CreateWithMissingRequiredFields("entry1", "blogPost"),
            EntryBuilder.CreateWithMissingRequiredFields("entry2", "blogPost")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(invalidEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Empty(result.ValidEntries);
        Assert.NotEmpty(result.Errors);
        Assert.All(result.Errors, error => Assert.Equal(ValidationErrorType.RequiredFieldMissing, error.Type));
    }

    [Fact]
    public async Task GivenEntriesWithDuplicateIds_WhenValidated_ThenDuplicateErrorsAreReported()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var entriesWithDuplicateIds = new[]
        {
            EntryBuilder.CreateBlogPost("duplicate-id"),
            EntryBuilder.CreateBlogPost("duplicate-id") // Same ID
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entriesWithDuplicateIds),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Single(result.ValidEntries); // First entry is valid
        Assert.Single(result.Errors); // Second entry has duplicate ID error
        Assert.Equal(ValidationErrorType.DuplicateId, result.Errors[0].Type);
        Assert.Equal(1, result.Errors[0].EntryIndex); // Second entry (index 1)
    }

    [Fact]
    public async Task GivenEntriesWithInvalidFields_WhenValidated_ThenInvalidFieldErrorsAreReported()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var entriesWithInvalidFields = new[]
        {
            EntryBuilder.CreateWithInvalidFields("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entriesWithInvalidFields),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Empty(result.ValidEntries);
        Assert.NotEmpty(result.Errors);
        
        var invalidFieldErrors = result.Errors.Where(e => e.Type == ValidationErrorType.InvalidField).ToList();
        Assert.Equal(2, invalidFieldErrors.Count); // Should have 2 invalid field errors
        Assert.Contains(invalidFieldErrors, e => e.FieldId == "invalidField");
        Assert.Contains(invalidFieldErrors, e => e.FieldId == "anotherInvalidField");
    }

    [Fact]
    public async Task GivenEntriesWithMissingIds_WhenValidated_ThenWarningsAreGenerated()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var entriesWithoutIds = new[]
        {
            EntryBuilder.CreateWithoutId("blogPost")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entriesWithoutIds),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Single(result.ValidEntries); // Entry is still valid, just has warning
        Assert.Empty(result.Errors);
        Assert.NotEmpty(result.Warnings);
        Assert.All(result.Warnings, warning => Assert.Equal(ValidationErrorType.RequiredFieldMissing, warning.Type));
    }

    [Fact]
    public async Task GivenMixedValidAndInvalidEntries_WhenValidated_ThenCorrectValidationSummaryIsReturned()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var mixedEntries = new[]
        {
        EntryBuilder.CreateBlogPost("valid1"),                                      // Valid
            EntryBuilder.CreateWithMissingRequiredFields("invalid1", "blogPost"),   // Invalid - missing required fields
            EntryBuilder.CreateBlogPost("valid2"),                                  // Valid
            EntryBuilder.CreateWithInvalidFields("invalid2"),                       // Invalid - has invalid fields
            EntryBuilder.CreateWithoutId("blogPost")                                // Valid but with warnings
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(mixedEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.ValidEntries.Count); // 3 valid entries (including one with warnings)
        Assert.NotEmpty(result.Errors); // Errors from invalid entries
        Assert.NotEmpty(result.Warnings); // Warnings from entry without ID
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenValidated_ThenEmptySummaryIsReturned()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var emptyEntries = Array.Empty<Contentful.Core.Models.Entry<dynamic>>();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(emptyEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Empty(result.ValidEntries);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenValidationIsCancelled_ThenOperationCancelledExceptionIsThrown()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _validator.ValidateAsync(input, cts.Token));
    }

    [Fact]
    public async Task GivenValidatorImplementsInterface_WhenChecked_ThenImplementsIContentUploadEntryValidator()
    {
        // Arrange & Act
        var implementsInterface = _validator is IContentUploadEntryValidator;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public async Task GivenNullContentType_WhenValidated_ThenNullReferenceExceptionIsThrown()
    {
        // Arrange
        _mockContentfulService.SetupContentType(null!); // Null content type
        _mockContentfulService.SetupLocales(LocaleBuilder.CreateMultiple().ToArray());

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "nonexistent",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _validator.ValidateAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenValidated_ThenAllEntriesAreProcessed()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _mockContentfulService.SetupContentType(contentType);
        _mockContentfulService.SetupLocales(locales.ToArray());

        const int entryCount = 100;
        var entries = Enumerable.Range(0, entryCount)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}"))
            .ToArray();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _validator.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Equal(entryCount, result.ValidEntries.Count);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }
}