using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulServiceBuilderTests
{
    private readonly ContentfulServiceBuilder _sut;
    
    private readonly MockContentfulClientManagementAdapterFactory _adapterFactoryMock = new();
    private readonly MockContentfulServiceOperationServiceFactory _operationServiceFactoryMock = new();
    private readonly MockRuntimeContext _runtimeContextMock = new();

    public ContentfulServiceBuilderTests()
    {
        _sut = new ContentfulServiceBuilder(
            _adapterFactoryMock,
            _operationServiceFactoryMock,
            _runtimeContextMock);
    }

    [Fact]
    public void GivenValidSpaceAndEnvironment_WhenBuild_ThenReturnsContentfulService()
    {
        // Arrange
        const string spaceId = "test-space-id";
        const string environmentId = "test-environment-id";

        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        var service = _sut.Build(spaceId, environmentId);

        // Assert
        Assert.NotNull(service);
        Assert.IsType<ContentfulService>(service);
        Assert.True(_adapterFactoryMock.CreateWasCalled);
        Assert.True(_operationServiceFactoryMock.CreateWasCalled);
    }

    [Fact]
    public void GivenValidSpaceAndEnvironment_WhenBuild_ThenPassesCorrectParameters()
    {
        // Arrange
        var spaceId = "test-space-id";
        var environmentId = "test-environment-id";

        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        _sut.Build(spaceId, environmentId);

        // Assert
        Assert.Equal(spaceId, _adapterFactoryMock.LastSpaceId);
        Assert.Equal(environmentId, _adapterFactoryMock.LastEnvironmentId);
        Assert.Equal(spaceId, _operationServiceFactoryMock.LastSpaceId);
        Assert.Equal(environmentId, _operationServiceFactoryMock.LastEnvironmentId);
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
        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        var service1 = _sut.Build("space1", "env1");
        var service2 = _sut.Build("space2", "env2");

        // Assert
        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.NotSame(service1, service2);
        Assert.Equal(2, _adapterFactoryMock.CreateCallCount);
        Assert.Equal(2, _operationServiceFactoryMock.CreateCallCount);
    }

    [Fact]
    public void GivenDifferentSpaceIds_WhenBuild_ThenCreatesServicesWithCorrectSpaceIds()
    {
        // Arrange
        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        _sut.Build("space1", "env1");
        _sut.Build("space2", "env2");

        // Assert
        Assert.Equal("space2", _adapterFactoryMock.LastSpaceId); // Last call
        Assert.Equal("env2", _adapterFactoryMock.LastEnvironmentId); // Last call
        Assert.Equal("space2", _operationServiceFactoryMock.LastSpaceId); // Last call
        Assert.Equal("env2", _operationServiceFactoryMock.LastEnvironmentId); // Last call
    }

    [Fact]
    public void GivenEmptySpaceId_WhenBuild_ThenStillCreatesService()
    {
        // Arrange
        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        var service = _sut.Build("", "environment");

        // Assert
        Assert.NotNull(service);
        Assert.Equal("", _adapterFactoryMock.LastSpaceId);
        Assert.Equal("environment", _adapterFactoryMock.LastEnvironmentId);
    }

    [Fact]
    public void GivenEmptyEnvironmentId_WhenBuild_ThenStillCreatesService()
    {
        // Arrange
        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        var service = _sut.Build("space", "");

        // Assert
        Assert.NotNull(service);
        Assert.Equal("space", _adapterFactoryMock.LastSpaceId);
        Assert.Equal("", _adapterFactoryMock.LastEnvironmentId);
    }

    [Fact]
    public void GivenSpecialCharactersInIds_WhenBuild_ThenHandlesCorrectly()
    {
        // Arrange
        var spaceId = "test-space_id.123";
        var environmentId = "test-env_id.456";

        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        var service = _sut.Build(spaceId, environmentId);

        // Assert
        Assert.NotNull(service);
        Assert.Equal(spaceId, _adapterFactoryMock.LastSpaceId);
        Assert.Equal(environmentId, _adapterFactoryMock.LastEnvironmentId);
    }

    [Fact]
    public void GivenBuilder_WhenBuildCalled_ThenCreatesFactoryClients()
    {
        // Arrange
        _adapterFactoryMock.SetupClient(new MockContentfulClient());
        _operationServiceFactoryMock.SetupClient(new MockContentfulManagementClient());

        // Act
        var service = _sut.Build("space", "env");

        // Assert
        Assert.NotNull(service);
        Assert.True(_adapterFactoryMock.CreateWasCalled);
        Assert.True(_operationServiceFactoryMock.CreateWasCalled);
    }
}