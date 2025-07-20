using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulServiceBuilderTests
{
    private readonly ContentfulServiceBuilder _sut;
    
    private readonly Mock<IContentfulManagementClientAdapterFactory> _adapterFactoryMock = new();
    private readonly Mock<IContentfulEntryOperationServiceFactory> _entryOperationServiceFactoryMock = new();
    private readonly Mock<IContentfulEntryBulkOperationServiceFactory> _entryBulkOperationServiceFactoryMock = new();
    private readonly Mock<IRuntimeContext> _runtimeContextMock = new();

    public ContentfulServiceBuilderTests()
    {
        _sut = new ContentfulServiceBuilder(
            _adapterFactoryMock.Object,
            _entryOperationServiceFactoryMock.Object,
            _entryBulkOperationServiceFactoryMock.Object,
            _runtimeContextMock.Object);
    }

    [Fact]
    public void GivenValidSpaceAndEnvironment_WhenBuild_ThenReturnsContentfulService()
    {
        // Arrange
        const string spaceId = "test-space-id";
        const string environmentId = "test-environment-id";

        // Act
        var service = _sut.WithSpaceId(spaceId)
            .WithEnvironmentId(environmentId)
            .Build();

        // Assert
        Assert.NotNull(service);
        Assert.IsAssignableFrom<IContentfulService>(service);
        
        // Verify the factory methods are called correctly
        _adapterFactoryMock.Verify(x => x.Create(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()), Times.Once);
        _entryOperationServiceFactoryMock.Verify(x => x.Create(It.IsAny<IContentfulManagementClientAdapter>()), Times.Once);
    }

    [Fact]
    public void GivenValidSpaceAndEnvironment_WhenBuild_ThenPassesCorrectParameters()
    {
        // Arrange
        var spaceId = "test-space-id";
        var environmentId = "test-environment-id";

        // MockLite limitation: Cannot use custom mock classes that were deleted
        // Simply verify the factory methods are called correctly

        // Act
        _sut.Build(spaceId, environmentId);

        // Assert
        _adapterFactoryMock.Verify(x => x.Create(spaceId, environmentId, It.IsAny<bool>()), Times.Once);
        _entryOperationServiceFactoryMock.Verify(x => x.Create(It.IsAny<IContentfulManagementClientAdapter>()), Times.Once);
        // Note: MockLite doesn't support LastSpaceId, LastEnvironmentId property tracking like custom mocks
    }

    [Fact]
    public void GivenBuilder_WhenCheckingInterface_ThenImplementsIContentfulServiceBuilder()
    {
        // Arrange & Act
        var implementsInterface = _sut is IContentfulServiceBuilder;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public void GivenMultipleCalls_WhenBuild_ThenCreatesMultipleServices()
    {
        // Arrange
        // MockLite limitation: Cannot use custom mock classes that were deleted
        // Simply verify the factory methods are called correctly

        // Act
        var service1 = _sut.Build("space1", "env1");
        var service2 = _sut.Build("space2", "env2");

        // Assert
        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.NotSame(service1, service2);
        _adapterFactoryMock.Verify(x => x.Create(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()), Times.Exactly(2));
        _entryOperationServiceFactoryMock.Verify(x => x.Create(It.IsAny<IContentfulManagementClientAdapter>()), Times.Exactly(2));
        // Note: MockLite doesn't support CreateCallCount property tracking like custom mocks
    }

    [Fact]
    public void GivenDifferentSpaceIds_WhenBuild_ThenCreatesServicesWithCorrectSpaceIds()
    {
        // Arrange
        // MockLite limitation: Cannot use custom mock classes that were deleted
        // Simply verify the factory methods are called correctly

        // Act
        _sut.Build("space1", "env1");
        _sut.Build("space2", "env2");

        // Assert
        _adapterFactoryMock.Verify(x => x.Create("space1", "env1", It.IsAny<bool>()), Times.Once);
        _adapterFactoryMock.Verify(x => x.Create("space2", "env2", It.IsAny<bool>()), Times.Once);
        _entryOperationServiceFactoryMock.Verify(x => x.Create(It.IsAny<IContentfulManagementClientAdapter>()), Times.Exactly(2));
    }

    [Fact]
    public void GivenEmptySpaceId_WhenBuild_ThenStillCreatesService()
    {
        // Arrange
        // MockLite limitation: Cannot use custom mock classes that were deleted
        // Simply verify the factory methods are called correctly

        // Act
        var service = _sut.Build("", "environment");

        // Assert
        Assert.NotNull(service);
        _adapterFactoryMock.Verify(x => x.Create("", "environment", It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public void GivenEmptyEnvironmentId_WhenBuild_ThenStillCreatesService()
    {
        // Arrange
        // MockLite limitation: Cannot use custom mock classes that were deleted
        // Simply verify the factory methods are called correctly

        // Act
        var service = _sut.Build("space", "");

        // Assert
        Assert.NotNull(service);
        _adapterFactoryMock.Verify(x => x.Create("space", "", It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public void GivenSpecialCharactersInIds_WhenBuild_ThenHandlesCorrectly()
    {
        // Arrange
        var spaceId = "test-space_id.123";
        var environmentId = "test-env_id.456";

        // MockLite limitation: Cannot use custom mock classes that were deleted
        // Simply verify the factory methods are called correctly

        // Act
        var service = _sut.Build(spaceId, environmentId);

        // Assert
        Assert.NotNull(service);
        _adapterFactoryMock.Verify(x => x.Create(spaceId, environmentId, It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public void GivenBuilder_WhenBuildCalled_ThenCreatesFactoryClients()
    {
        // Arrange
        // MockLite limitation: Cannot use custom mock classes that were deleted
        // Simply verify the factory methods are called correctly

        // Act
        var service = _sut.Build("space", "env");

        // Assert
        Assert.NotNull(service);
        _adapterFactoryMock.Verify(x => x.Create(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()), Times.Once);
        _entryOperationServiceFactoryMock.Verify(x => x.Create(It.IsAny<IContentfulManagementClientAdapter>()), Times.Once);
    }
}