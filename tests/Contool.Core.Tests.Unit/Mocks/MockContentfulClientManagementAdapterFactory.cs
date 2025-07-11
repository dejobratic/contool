using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Tests.Unit.Mocks;

internal class MockContentfulClientManagementAdapterFactory : IContentfulManagementClientAdapterFactory
{
    private MockContentfulClient? _client;

    public bool CreateWasCalled { get; private set; }
    public string? LastSpaceId { get; private set; }
    public string? LastEnvironmentId { get; private set; }
    public bool LastUsePreviewApi { get; private set; }
    public int CreateCallCount { get; private set; }

    public void SetupClient(MockContentfulClient client)
    {
        _client = client;
    }

    public IContentfulManagementClientAdapter Create(string? spaceId, string? environmentId, bool usePreviewApi)
    {
        CreateWasCalled = true;
        LastSpaceId = spaceId;
        LastEnvironmentId = environmentId;
        LastUsePreviewApi = usePreviewApi;
        CreateCallCount++;
        
        return new MockContentfulManagementClientAdapter();
    }
}