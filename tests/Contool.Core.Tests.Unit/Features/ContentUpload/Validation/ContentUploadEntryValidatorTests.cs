using Contool.Core.Features.ContentUpload;
using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Validation;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;
// ReSharper disable ConvertTypeCheckToNullCheck

namespace Contool.Core.Tests.Unit.Features.ContentUpload.Validation;

public class ContentUploadEntryValidatorTests
{
    private readonly ContentUploadEntryValidator _sut;
    
    private readonly Mock<IContentfulService> _contentfulServiceMock = new();

    public ContentUploadEntryValidatorTests()
    {
        _contentfulServiceMock.SetupDefaults();
        
        _sut = new ContentUploadEntryValidator();
    }

    [Fact]
    public async Task GivenValidEntries_WhenValidated_ThenAllEntriesAreValid()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        var validEntries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(validEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        var invalidEntries = new[]
        {
            EntryBuilder.CreateWithMissingRequiredFields("entry1"),
            EntryBuilder.CreateWithMissingRequiredFields("entry2")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(invalidEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        var entriesWithDuplicateIds = new[]
        {
            EntryBuilder.CreateBlogPost("duplicate-id"),
            EntryBuilder.CreateBlogPost("duplicate-id") // Same ID
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entriesWithDuplicateIds),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Single(result.ValidEntries); // The first entry is valid
        Assert.Single(result.Errors); // The second entry has duplicate ID error
        Assert.Equal(ValidationErrorType.DuplicateId, result.Errors[0].Type);
        Assert.Equal(1, result.Errors[0].EntryIndex); // Second entry (index 1)
    }

    [Fact]
    public async Task GivenEntriesWithInvalidFields_WhenValidated_ThenInvalidFieldErrorsAreReported()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        var entriesWithInvalidFields = new[]
        {
            EntryBuilder.CreateWithInvalidFields("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entriesWithInvalidFields),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        var entriesWithoutIds = new[]
        {
            EntryBuilder.CreateWithoutId()
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entriesWithoutIds),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Single(result.ValidEntries); // Entry is still valid, just has a warning
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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        var mixedEntries = new[]
        {
        EntryBuilder.CreateBlogPost("valid1"),                                      // Valid
            EntryBuilder.CreateWithMissingRequiredFields("invalid1"),   // Invalid - missing required fields
            EntryBuilder.CreateBlogPost("valid2"),                                  // Valid
            EntryBuilder.CreateWithInvalidFields("invalid2"),                       // Invalid - has invalid fields
            EntryBuilder.CreateWithoutId()                                // Valid but with warnings
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(mixedEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        var emptyEntries = Array.Empty<Contentful.Core.Models.Entry<dynamic>>();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(emptyEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Empty(result.ValidEntries);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void GivenValidatorImplementsInterface_WhenChecked_ThenImplementsIContentUploadEntryValidator()
    {
        // Arrange & Act & Assert
        Assert.True(_sut is IContentUploadEntryValidator);
    }

    [Fact]
    public async Task GivenNullContentType_WhenValidated_ThenNullReferenceExceptionIsThrown()
    {
        // Arrange
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(LocaleBuilder.CreateMultiple().ToArray());

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "nonexistent",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _sut.ValidateAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenValidated_ThenAllEntriesAreProcessed()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales.ToArray());

        const int entryCount = 100;
        var entries = Enumerable.Range(0, entryCount)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}"))
            .ToArray();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Contentful.Core.Models.Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        var result = await _sut.ValidateAsync(input, CancellationToken.None);

        // Assert
        Assert.Equal(entryCount, result.ValidEntries.Count);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }
}