using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationServiceTests
{
    private readonly ContentfulEntryBulkOperationService _sut;
    
    private readonly Mock<HttpClient> _httpClientMock = new();

    public ContentfulEntryBulkOperationServiceTests()
    {
        var options = new ContentfulOptions
        {
            SpaceId = "test-space",
            Environment = "test-env", 
            ManagementApiKey = "test-key"
        };
        
        _sut = new ContentfulEntryBulkOperationService(
            _httpClientMock.Object,
            options);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenPublishEntriesAsync_ThenReturnsEmptyResults()
    {
        // Arrange
        var entries = Array.Empty<Entry<dynamic>>();

        // Act
        var actual = await _sut.PublishEntriesAsync(entries);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenUnpublishEntriesAsync_ThenReturnsEmptyResults()
    {
        // Arrange
        var entries = Array.Empty<Entry<dynamic>>();

        // Act
        var actual = await _sut.UnpublishEntriesAsync(entries);

        // Assert
        Assert.Empty(actual);
    }
}