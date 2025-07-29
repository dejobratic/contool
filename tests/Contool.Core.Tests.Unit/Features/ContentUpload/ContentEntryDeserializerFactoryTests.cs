using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Features.ContentUpload;

public class ContentEntryDeserializerFactoryTests
{
    private readonly ContentEntryDeserializerFactory _sut = new();
    
    private readonly Mock<IContentfulService> _contentfulServiceMock = new();

    [Fact]
    public async Task GivenValidContentTypeId_WhenCreateAsync_ThenReturnsDeserializer()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("blogPost", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act
        var deserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock.Object, CancellationToken.None);

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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("blogPost", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act
        await _sut.CreateAsync("blogPost", _contentfulServiceMock.Object, CancellationToken.None);

        // Assert
        // The service should have been called to get the content type
        _contentfulServiceMock.Verify(x => x.GetContentTypeAsync("blogPost", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenCreateAsync_ThenFetchesLocalesFromService()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("blogPost", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act
        await _sut.CreateAsync("blogPost", _contentfulServiceMock.Object, CancellationToken.None);

        // Assert
        // The service should have been called to get the locales
        _contentfulServiceMock.Verify(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("blogPost", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act
        var deserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(deserializer);
        Assert.IsType<ContentEntryDeserializer>(deserializer);
    }

    [Fact]
    public async Task GivenDifferentContentTypes_WhenCreateAsync_ThenCreatesCorrectDeserializers()
    {
        // Arrange
        var blogPostType = ContentTypeBuilder.CreateBlogPost();
        var productType = ContentTypeBuilder.CreateProduct();
        var locales = new[] { LocaleBuilder.CreateDefault() };
        
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act & Assert - Blog Post
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("blogPost", It.IsAny<CancellationToken>()))
            .ReturnsAsync(blogPostType);
        var blogDeserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock.Object, CancellationToken.None);
        Assert.NotNull(blogDeserializer);

        // Act & Assert - Product
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("product", It.IsAny<CancellationToken>()))
            .ReturnsAsync(productType);
        var productDeserializer = await _sut.CreateAsync("product", _contentfulServiceMock.Object, CancellationToken.None);
        Assert.NotNull(productDeserializer);
    }

    [Fact]
    public async Task GivenEmptyLocales_WhenCreateAsync_ThenCreatesDeserializerWithEmptyLocales()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var emptyLocales = Array.Empty<Locale>();
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("blogPost", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyLocales);

        // Act
        var deserializer = await _sut.CreateAsync("blogPost", _contentfulServiceMock.Object, CancellationToken.None);

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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("minimal", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act
        var deserializer = await _sut.CreateAsync("minimal", _contentfulServiceMock.Object, CancellationToken.None);

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
        
        _contentfulServiceMock.Setup(x => x.GetContentTypeAsync("complex", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _contentfulServiceMock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act
        var deserializer = await _sut.CreateAsync("complex", _contentfulServiceMock.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(deserializer);
        Assert.IsType<ContentEntryDeserializer>(deserializer);
    }
}