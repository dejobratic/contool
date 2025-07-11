using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Tests.Unit.Helpers;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Tests.Unit.Features.ContentUpload;

public class ContentEntryDeserializerTests
{
    private readonly ContentEntryDeserializer _deserializer;
    private readonly ContentType _contentType;
    private readonly ContentLocales _locales;

    public ContentEntryDeserializerTests()
    {
        _contentType = ContentTypeBuilder.CreateBlogPost();
        _locales = ContentLocalesBuilder.CreateDefault();
        _deserializer = new ContentEntryDeserializer(_contentType, _locales);
    }

    [Fact]
    public void GivenValidRowWithRequiredFields_WhenDeserialize_ThenCreatesCorrectEntry()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            ["title"] = "Test Blog Post",
            ["content"] = "This is test content",
            ["publishDate"] = "2024-01-01T00:00:00Z"
        };

        // Act
        var entry = _deserializer.Deserialize(row);

        // Assert
        Assert.NotNull(entry);
        Assert.NotNull(entry.Fields);
        Assert.NotNull(entry.SystemProperties);
        Assert.NotNull(entry.Metadata);
        
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Equal("Test Blog Post", fields["title"]?["en-US"]?.Value<string>());
        Assert.Equal("This is test content", fields["content"]?["en-US"]?.Value<string>());
        Assert.Equal("2024-01-01T00:00:00Z", fields["publishDate"]?["en-US"]?.Value<string>());
    }

    [Fact]
    public void GivenRowWithSystemProperties_WhenDeserialize_ThenSetsSystemPropertiesCorrectly()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            ["sys.id"] = "test-entry-id",
            ["sys.contentType"] = "blogPost",
            ["sys.version"] = 1,
            ["title"] = "Test Blog Post"
        };

        // Act
        var entry = _deserializer.Deserialize(row);

        // Assert
        Assert.Equal("test-entry-id", entry.SystemProperties.Id);
        Assert.Equal("blogPost", entry.SystemProperties.ContentType.SystemProperties.Id);
        Assert.Equal(1, entry.SystemProperties.Version);
    }

    [Fact]
    public void GivenRowWithLocalizedFields_WhenDeserialize_ThenSetsLocalizedContent()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            ["title"] = "English Title",
            ["title.es"] = "Título en Español",
            ["content"] = "English content",
            ["content.es"] = "Contenido en español"
        };

        // Act
        var entry = _deserializer.Deserialize(row);

        // Assert
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Equal("English Title", fields["title"]?["en-US"]?.Value<string>());
        Assert.Equal("Título en Español", fields["title"]?["es"]?.Value<string>());
        Assert.Equal("English content", fields["content"]?["en-US"]?.Value<string>());
        Assert.Equal("Contenido en español", fields["content"]?["es"]?.Value<string>());
    }

    [Fact]
    public void GivenRowWithUnknownFields_WhenDeserialize_ThenIgnoresUnknownFields()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            ["title"] = "Test Blog Post",
            ["unknownField"] = "This should be ignored",
            ["anotherUnknownField"] = "This too"
        };

        // Act
        var entry = _deserializer.Deserialize(row);

        // Assert
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Equal("Test Blog Post", fields["title"]?["en-US"]?.Value<string>());
        Assert.Null(fields["unknownField"]);
        Assert.Null(fields["anotherUnknownField"]);
    }

    [Fact]
    public void GivenRowWithNullValues_WhenDeserialize_ThenHandlesNullsCorrectly()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            ["title"] = "Test Blog Post",
            ["content"] = null,
            ["publishDate"] = null
        };

        // Act
        var entry = _deserializer.Deserialize(row);

        // Assert
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Equal("Test Blog Post", fields["title"]?["en-US"]?.Value<string>());
        Assert.Null(fields["content"]?["en-US"]);
        Assert.Null(fields["publishDate"]?["en-US"]);
    }

    [Fact]
    public void GivenEmptyRow_WhenDeserialize_ThenCreatesEmptyEntry()
    {
        // Arrange
        var row = new Dictionary<string, object?>();

        // Act
        var entry = _deserializer.Deserialize(row);

        // Assert
        Assert.NotNull(entry);
        Assert.NotNull(entry.Fields);
        Assert.NotNull(entry.SystemProperties);
        Assert.NotNull(entry.Metadata);
        
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Empty(fields);
    }

    [Fact]
    public void GivenRowWithMultipleLocales_WhenDeserialize_ThenUsesCorrectDefaultLocale()
    {
        // Arrange
        var customLocales = new ContentLocales(
        [
            new Locale { Code = "es", Default = true },
            new Locale { Code = "en", Default = false },
            new Locale { Code = "fr", Default = true },
        ]);
        var deserializer = new ContentEntryDeserializer(_contentType, customLocales);
        
        var row = new Dictionary<string, object?>
        {
            ["title"] = "Default Title" // Should use Spanish as default
        };

        // Act
        var entry = deserializer.Deserialize(row);

        // Assert
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Equal("Default Title", fields["title"]?["es"]?.Value<string>());
        Assert.Null(fields["title"]?["en"]);
    }

    [Fact]
    public void GivenRowWithBooleanField_WhenDeserialize_ThenHandlesBooleanCorrectly()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateWithBooleanField();
        var deserializer = new ContentEntryDeserializer(contentType, _locales);
        
        var row = new Dictionary<string, object?>
        {
            ["title"] = "Test Post",
            ["isPublished"] = true
        };

        // Act
        var entry = deserializer.Deserialize(row);

        // Assert
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Equal("Test Post", fields["title"]?["en-US"]?.Value<string>());
        Assert.True(fields["isPublished"]?["en-US"]?.Value<bool>());
    }

    [Fact]
    public void GivenRowWithNumericField_WhenDeserialize_ThenHandlesNumbersCorrectly()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateWithNumericField();
        var deserializer = new ContentEntryDeserializer(contentType, _locales);
        
        var row = new Dictionary<string, object?>
        {
            ["title"] = "Test Post",
            ["viewCount"] = 100
        };

        // Act
        var entry = deserializer.Deserialize(row);

        // Assert
        var fields = entry.Fields as JObject;
        Assert.NotNull(fields);
        Assert.Equal("Test Post", fields["title"]?["en-US"]?.Value<string>());
        Assert.Equal(100, fields["viewCount"]?["en-US"]?.Value<int>());
    }

    [Fact]
    public void GivenInvalidRow_WhenDeserialize_ThenThrowsArgumentException()
    {
        // Arrange
        var invalidRow = "not a dictionary";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _deserializer.Deserialize(invalidRow));
        Assert.Contains("Row must be a dictionary", exception.Message);
    }

    [Fact]
    public void GivenDeserializer_WhenCheckingInterface_ThenImplementsIContentEntryDeserializer()
    {
        // Arrange & Act
        var implementsInterface = _deserializer is IContentEntryDeserializer;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public void GivenRowWithComplexSystemProperties_WhenDeserialize_ThenSetsAllSystemProperties()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            ["sys.id"] = "test-entry-id",
            ["sys.contentType"] = "blogPost",
            ["sys.version"] = 5,
            ["sys.createdAt"] = "2024-01-01T10:00:00Z",
            ["sys.updatedAt"] = "2024-01-02T15:30:00Z",
            ["title"] = "Test Blog Post"
        };

        // Act
        var entry = _deserializer.Deserialize(row);

        // Assert
        Assert.Equal("test-entry-id", entry.SystemProperties.Id);
        Assert.Equal("blogPost", entry.SystemProperties.ContentType.SystemProperties.Id);
        Assert.Equal(5, entry.SystemProperties.Version);
        Assert.Equal("2024-01-01T10:00:00Z", entry.SystemProperties.CreatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        Assert.Equal("2024-01-02T15:30:00Z", entry.SystemProperties.UpdatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"));
    }
}