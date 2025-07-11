using Contentful.Core.Models;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Validation;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentUpload.Validation;

public class ContentUploaderValidationDecoratorTests
{
    private readonly ContentUploaderValidationDecorator _decorator;
    private readonly MockContentUploader _mockUploader;
    private readonly MockContentUploadEntryValidator _mockValidator;
    private readonly MockContentfulService _mockContentfulService;

    public ContentUploaderValidationDecoratorTests()
    {
        _mockUploader = new MockContentUploader();
        _mockValidator = new MockContentUploadEntryValidator();
        _decorator = new ContentUploaderValidationDecorator(_mockUploader, _mockValidator);
        _mockContentfulService = new MockContentfulService();
    }

    [Fact]
    public async Task GivenValidEntries_WhenUpload_ThenCallsInnerUploaderWithAllEntries()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        var validationSummary = new EntryValidationSummary();
        foreach (var entry in entries)
        {
            validationSummary.ValidEntries.Add(entry);
        }
        
        _mockValidator.SetupValidationResult(validationSummary);

        // Act
        await _decorator.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockValidator.ValidateAsyncWasCalled);
        Assert.True(_mockUploader.UploadAsyncWasCalled);
        Assert.Equal(entries.Length, _mockUploader.LastInput?.Entries.Total);
    }

    [Fact]
    public async Task GivenEntriesWithErrors_WhenUploadOnlyValidEntriesFalse_ThenThrowsValidationException()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateWithMissingRequiredFields("entry2", "blogPost")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        var validationSummary = new EntryValidationSummary();
        validationSummary.ValidEntries.Add(entries[0]); // Only the first entry is valid
        validationSummary.Errors.Add(ValidationErrorBuilder.CreateRequiredFieldMissing(1, "title"));
        
        _mockValidator.SetupValidationResult(validationSummary);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _decorator.UploadAsync(input, CancellationToken.None));
        
        Assert.Contains("validation errors", exception.Message);
        Assert.False(_mockUploader.UploadAsyncWasCalled);
    }

    [Fact]
    public async Task GivenEntriesWithErrors_WhenUploadOnlyValidEntriesTrue_ThenUploadsOnlyValidEntries()
    {
        // Arrange
        var validEntry = EntryBuilder.CreateBlogPost("entry1");
        var invalidEntry = EntryBuilder.CreateWithMissingRequiredFields("entry2", "blogPost");
        var entries = new[] { validEntry, invalidEntry };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = true,
            PublishEntries = false
        };

        var validationSummary = new EntryValidationSummary();
        validationSummary.ValidEntries.Add(validEntry); // Only first entry is valid
        validationSummary.Errors.Add(ValidationErrorBuilder.CreateRequiredFieldMissing(1, "title"));
        
        _mockValidator.SetupValidationResult(validationSummary);

        // Act
        await _decorator.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockValidator.ValidateAsyncWasCalled);
        Assert.True(_mockUploader.UploadAsyncWasCalled);
        Assert.Equal(1, _mockUploader.LastInput?.Entries.Total); // Only valid entries
    }

    [Fact]
    public async Task GivenEntriesWithWarnings_WhenUpload_ThenProceeds()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateWithoutId("blogPost") // Entry without ID generates warning
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        var validationSummary = new EntryValidationSummary();
        foreach (var entry in entries)
        {
            validationSummary.ValidEntries.Add(entry);
        }
        validationSummary.Warnings.Add(ValidationWarningBuilder.CreateMissingId(1));
        
        _mockValidator.SetupValidationResult(validationSummary);

        // Act
        await _decorator.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockValidator.ValidateAsyncWasCalled);
        Assert.True(_mockUploader.UploadAsyncWasCalled);
        Assert.Equal(entries.Length, _mockUploader.LastInput?.Entries.Total);
    }

    [Fact]
    public async Task GivenAllEntriesInvalid_WhenUploadOnlyValidEntriesTrue_ThenUploadsEmptyCollection()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateWithMissingRequiredFields("entry1", "blogPost"),
            EntryBuilder.CreateWithMissingRequiredFields("entry2", "blogPost")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = true,
            PublishEntries = false
        };

        var validationSummary = new EntryValidationSummary();
        validationSummary.Errors.Add(ValidationErrorBuilder.CreateRequiredFieldMissing(0, "title"));
        validationSummary.Errors.Add(ValidationErrorBuilder.CreateRequiredFieldMissing(1, "title"));
        
        _mockValidator.SetupValidationResult(validationSummary);

        // Act
        await _decorator.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockValidator.ValidateAsyncWasCalled);
        Assert.True(_mockUploader.UploadAsyncWasCalled);
        Assert.Equal(0, _mockUploader.LastInput?.Entries.Total); // No valid entries
    }

    [Fact]
    public async Task GivenCancellationToken_WhenCancelled_ThenPassesToValidator()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _decorator.UploadAsync(input, cts.Token));
        
        Assert.True(_mockValidator.ValidateAsyncWasCalled);
        Assert.True(_mockValidator.LastCancellationToken.IsCancellationRequested);
    }

    [Fact]
    public void GivenDecorator_WhenCheckingInterface_ThenImplementsIContentUploader()
    {
        // Arrange & Act
        var implementsInterface = _decorator is IContentUploader;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public async Task GivenValidatorThrowsException_WhenUpload_ThenPropagatesException()
    {
        // Arrange
        _mockValidator.ShouldThrowException = true;

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _decorator.UploadAsync(input, CancellationToken.None));
        
        Assert.True(_mockValidator.ValidateAsyncWasCalled);
        Assert.False(_mockUploader.UploadAsyncWasCalled);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenUpload_ThenStillValidatesAndUploads()
    {
        // Arrange
        var emptyEntries = Array.Empty<Entry<dynamic>>();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(emptyEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        var validationSummary = new EntryValidationSummary(); // Empty summary
        _mockValidator.SetupValidationResult(validationSummary);

        // Act
        await _decorator.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockValidator.ValidateAsyncWasCalled);
        Assert.True(_mockUploader.UploadAsyncWasCalled);
        Assert.Equal(0, _mockUploader.LastInput?.Entries.Total);
    }

    [Fact]
    public async Task GivenDecorator_WhenUpload_ThenPreservesInputProperties()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "specialContentType",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = true
        };

        var validationSummary = new EntryValidationSummary();
        validationSummary.ValidEntries.Add(entries[0]);
        _mockValidator.SetupValidationResult(validationSummary);

        // Act
        await _decorator.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockUploader.UploadAsyncWasCalled);
        var lastInput = _mockUploader.LastInput;
        Assert.NotNull(lastInput);
        Assert.Equal("specialContentType", lastInput.ContentTypeId);
        Assert.Same(_mockContentfulService, lastInput.ContentfulService);
        Assert.True(lastInput.PublishEntries);
        Assert.False(lastInput.UploadOnlyValidEntries); // This is used for decorator logic, not passed through
    }
}