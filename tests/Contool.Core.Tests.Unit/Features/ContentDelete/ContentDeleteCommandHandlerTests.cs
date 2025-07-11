using Contool.Core.Features.ContentDelete;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentDelete;

public class ContentDeleteCommandHandlerTests
{
    private readonly ContentDeleteCommandHandler _sut;
    
    private readonly MockContentfulServiceBuilder _contentfulServiceBuilderMock = new();
    private readonly MockContentDeleter _contentDeleterMock = new();

    public ContentDeleteCommandHandlerTests()
    {
        _contentfulServiceBuilderMock.SetupService(new MockContentfulService());
        _sut = new ContentDeleteCommandHandler(
            _contentfulServiceBuilderMock, 
            _contentDeleterMock);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenBuildsContentfulService()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        Assert.True(_contentfulServiceBuilderMock.BuildWasCalled);
        Assert.Equal(command.SpaceId, _contentfulServiceBuilderMock.LastSpaceId);
        Assert.Equal(command.EnvironmentId, _contentfulServiceBuilderMock.LastEnvironmentId);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenCallsContentDeleter()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        Assert.True(_contentDeleterMock.DeleteAsyncWasCalled);
        Assert.NotNull(_contentDeleterMock.LastInput);
        Assert.Equal(command.ContentTypeId, _contentDeleterMock.LastInput.ContentTypeId);
        Assert.Equal(command.IncludeArchived, _contentDeleterMock.LastInput.IncludeArchived);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenPassesCorrectInputToDeleter()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        Assert.True(_contentDeleterMock.DeleteAsyncWasCalled);
        Assert.NotNull(_contentDeleterMock.LastInput);
        Assert.Equal(command.ContentTypeId, _contentDeleterMock.LastInput.ContentTypeId);
        Assert.Equal(command.IncludeArchived, _contentDeleterMock.LastInput.IncludeArchived);
        Assert.NotNull(_contentDeleterMock.LastInput.ContentfulService);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenHandleAsync_ThenRespectsCancellation()
    {
        // Arrange
        var command = CreateDeleteCommand();
        
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.HandleAsync(command, cts.Token));
    }

    [Fact]
    public async Task GivenContentfulServiceBuilderThrowsException_WhenHandleAsync_ThenBubblesException()
    {
        // Arrange
        var command = CreateDeleteCommand();
        _contentfulServiceBuilderMock.SetupToThrow(new InvalidOperationException("Service builder failed"));
        
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
        _contentDeleterMock.SetupToThrow(new InvalidOperationException("Delete operation failed"));
        
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
        Assert.True(_contentDeleterMock.DeleteAsyncWasCalled);
        Assert.NotNull(_contentDeleterMock.LastInput);
        Assert.True(_contentDeleterMock.LastInput.IncludeArchived);
    }

    [Fact]
    public async Task GivenCommandWithIncludeArchivedFalse_WhenHandleAsync_ThenPassesIncludeArchivedToDeleter()
    {
        // Arrange
        var command = CreateDeleteCommand(includeArchived: false);
        
        // Act
        await _sut.HandleAsync(command, CancellationToken.None);
        
        // Assert
        Assert.True(_contentDeleterMock.DeleteAsyncWasCalled);
        Assert.NotNull(_contentDeleterMock.LastInput);
        Assert.False(_contentDeleterMock.LastInput.IncludeArchived);
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