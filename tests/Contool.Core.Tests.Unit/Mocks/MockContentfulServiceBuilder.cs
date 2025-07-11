using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentfulServiceBuilder : IContentfulServiceBuilder
{
    public bool BuildWasCalled { get; private set; }
    public string? LastSpaceId { get; private set; }
    public string? LastEnvironmentId { get; private set; }
    
    private IContentfulService? _service;

    public void SetupService(IContentfulService service)
    {
        _service = service;
    }

    public IContentfulServiceBuilder WithSpaceId(string? spaceId)
    {
        LastSpaceId = spaceId;
        return this;
    }

    public IContentfulServiceBuilder WithEnvironmentId(string? environmentId)
    {
        LastEnvironmentId = environmentId;
        return this;
    }

    public IContentfulServiceBuilder WithPreviewApi(bool usePreviewApi)
    {
        return this;
    }

    public IContentfulService Build()
    {
        BuildWasCalled = true;
        return _service ?? throw new InvalidOperationException("Service not set up");
    }
}