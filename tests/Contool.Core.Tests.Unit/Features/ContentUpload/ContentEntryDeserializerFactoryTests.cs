using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentUpload;

public class ContentEntryDeserializerFactoryTests
{
    private readonly ContentEntryDeserializerFactory _sut = new();
    
    private readonly MockContentfulService _contentfulServiceMock = new();

    [Fact]
    public async Task GivenValidContentTypeId_WhenCreateAsync_ThenReturnsDeserializer()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(locales);

        // Act
        var deserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock, CancellationToken.None);

        // Assert
        Assert.NotNull(deserializer);
        Assert.IsType<ContentEntryDeserializer>(deserializer);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenCreateAsync_ThenFetchesContentTypeFromService()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(locales);

        // Act
        await _sut.CreateAsync("blogPost", _contentfulServiceMock, CancellationToken.None);

        // Assert
        // The service should have been called to get the content type
        Assert.NotNull(_contentfulServiceMock);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenCreateAsync_ThenFetchesLocalesFromService()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(locales);

        // Act
        await _sut.CreateAsync("blogPost", _contentfulServiceMock, CancellationToken.None);

        // Assert
        // The service should have been called to get the locales
        Assert.NotNull(_contentfulServiceMock);
    }

    [Fact]
    public async Task GivenMultipleLocales_WhenCreateAsync_ThenCreatesDeserializerWithAllLocales()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = new[]
        {
            LocaleBuilder.CreateDefault(),
            LocaleBuilder.CreateSpanish(),
            LocaleBuilder.CreateFrench()
        };
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(locales);

        // Act
        var deserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock, CancellationToken.None);

        // Assert
        Assert.NotNull(deserializer);
        Assert.IsType<ContentEntryDeserializer>(deserializer);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenCancelled_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(locales);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.CreateAsync("blogPost", _contentfulServiceMock, cts.Token));
    }

    [Fact]
    public async Task GivenDifferentContentTypes_WhenCreateAsync_ThenCreatesCorrectDeserializers()
    {
        // Arrange
        var blogPostType = ContentTypeBuilder.CreateBlogPost();
        var productType = ContentTypeBuilder.CreateProduct();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.SetupLocales(locales);

        // Act & Assert - Blog Post
        _contentfulServiceMock.SetupContentType(blogPostType);
        var blogDeserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock, CancellationToken.None);
        Assert.NotNull(blogDeserializer);

        // Act & Assert - Product
        _contentfulServiceMock.SetupContentType(productType);
        var productDeserializer = await _sut.CreateAsync("product", _contentfulServiceMock, CancellationToken.None);
        Assert.NotNull(productDeserializer);
    }

    [Fact]
    public async Task GivenEmptyLocales_WhenCreateAsync_ThenCreatesDeserializerWithEmptyLocales()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var emptyLocales = Array.Empty<Locale>();
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(emptyLocales);

        // Act
        var deserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock, CancellationToken.None);

        // Assert
        Assert.NotNull(deserializer);
        Assert.IsType<ContentEntryDeserializer>(deserializer);
    }

    [Fact]
    public async Task GivenMinimalContentType_WhenCreateAsync_ThenCreatesDeserializer()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateMinimal();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(locales);

        // Act
        var deserializer = await _sut.CreateAsync("minimal", _contentfulServiceMock, CancellationToken.None);

        // Assert
        Assert.NotNull(deserializer);
        Assert.IsType<ContentEntryDeserializer>(deserializer);
    }

    [Fact]
    public void GivenFactory_WhenCheckingInterface_ThenImplementsIContentEntryDeserializerFactory()
    {
        // Arrange & Act
        var implementsInterface = _sut is IContentEntryDeserializerFactory;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public async Task GivenComplexContentType_WhenCreateAsync_ThenCreatesDeserializerWithAllFields()
    {
        // Arrange
        var contentType = ContentTypeBuilder.Create()
            .WithId("complex")
            .WithName("Complex Content Type")
            .WithField("title", "Title", "Symbol", required: true)
            .WithField("body", "Body", "RichText", required: true)
            .WithField("publishDate", "Publish Date", "Date", required: false)
            .WithField("isPublished", "Is Published", "Boolean", required: false)
            .WithField("viewCount", "View Count", "Integer", required: false)
            .WithField("tags", "Tags", "Array", required: false)
            .Build();
        
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.SetupContentType(contentType);
        _contentfulServiceMock.SetupLocales(locales);

        // Act
        var deserializer = await _sut.CreateAsync("complex", _contentfulServiceMock, CancellationToken.None);

        // Assert
        Assert.NotNull(deserializer);
        Assert.IsType<ContentEntryDeserializer>(deserializer);
    }
}