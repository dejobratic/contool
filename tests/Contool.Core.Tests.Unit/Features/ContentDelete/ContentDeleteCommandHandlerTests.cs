using Contool.Core.Features.ContentDelete;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Contool.Core.Tests.Unit.Features.ContentDelete;

public class ContentDeleteCommandHandlerTests
{
    private readonly ContentDeleteCommandHandler _sut;
    
    private readonly Mock<IContentfulServiceBuilder> _contentfulServiceBuilderMock = new();
    private readonly Mock<IContentfulService> _contentfulServiceMock = new();
    private readonly Mock<IContentDeleter> _contentDeleterMock = new();

    public ContentDeleteCommandHandlerTests()
    {
        _contentfulServiceBuilderMock.SetupDefaults(_contentfulServiceMock);
        _contentfulServiceMock.SetupDefaults();
        _contentDeleterMock.SetupDefaults();
        
        _sut = new ContentDeleteCommandHandler(
            _contentfulServiceBuilderMock.Object, 
            _contentDeleterMock.Object);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenBuildsContentfulService()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        _contentfulServiceBuilderMock.Verify(x => x.WithSpaceId(command.SpaceId), Times.Once);
        _contentfulServiceBuilderMock.Verify(x => x.WithEnvironmentId(command.EnvironmentId), Times.Once);
        _contentfulServiceBuilderMock.Verify(x => x.Build(), Times.Once);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenCallsContentDeleter()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        _contentDeleterMock.Verify(x => x.DeleteAsync(
            It.Is<ContentDeleterInput>(input => 
                input.ContentTypeId == command.ContentTypeId && 
                input.IncludeArchived == command.IncludeArchived &&
                input.ContentfulService == _contentfulServiceMock.Object), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenPassesCorrectInputToDeleter()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        _contentDeleterMock.Verify(x => x.DeleteAsync(
            It.Is<ContentDeleterInput>(input => 
                input.ContentTypeId == command.ContentTypeId && 
                input.IncludeArchived == command.IncludeArchived &&
                input.ContentfulService != null), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenContentfulServiceBuilderThrowsException_WhenHandleAsync_ThenBubblesException()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        _contentfulServiceBuilderMock.Setup(x => x.Build())
            .Throws(new InvalidOperationException("Service builder failed"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.HandleAsync(command, CancellationToken.None));
        
        Assert.Equal("Service builder failed", exception.Message);
    }

    [Fact]
    public async Task GivenContentDeleterThrowsException_WhenHandleAsync_ThenBubblesException()
    {
        // Arrange
        var command = CreateDeleteCommand();
        _contentDeleterMock.Setup(x => x.DeleteAsync(It.IsAny<ContentDeleterInput>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Delete operation failed"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.HandleAsync(command, CancellationToken.None));
        
        Assert.Equal("Delete operation failed", exception.Message);
    }

    [Fact]
    public async Task GivenCommandWithIncludeArchivedTrue_WhenHandleAsync_ThenPassesIncludeArchivedToDeleter()
    {
        // Arrange
        var command = CreateDeleteCommand(includeArchived: true);
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        _contentDeleterMock.Verify(x => x.DeleteAsync(
            It.Is<ContentDeleterInput>(input => input.IncludeArchived == true), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenCommandWithIncludeArchivedFalse_WhenHandleAsync_ThenPassesIncludeArchivedToDeleter()
    {
        // Arrange
        var command = CreateDeleteCommand(includeArchived: false);
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        _contentDeleterMock.Verify(x => x.DeleteAsync(
            It.Is<ContentDeleterInput>(input => input.IncludeArchived == false), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ContentDeleteCommand CreateDeleteCommand(bool includeArchived = false)
    {
        return new ContentDeleteCommand
        {
            SpaceId = "test-space-id",
            EnvironmentId = "test-environment-id",
            ContentTypeId = "test-content-type",
            IncludeArchived = includeArchived
        };
    }
}