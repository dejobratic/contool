using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Tests.Unit.Mocks;

internal class MockContentfulServiceOperationServiceFactory : IContentfulEntryOperationServiceFactory
{
    private MockContentfulManagementClient? _client;

    public bool CreateWasCalled { get; private set; }
    public string? LastSpaceId { get; private set; }
    public string? LastEnvironmentId { get; private set; }
    public int CreateCallCount { get; private set; }

    public void SetupClient(MockContentfulManagementClient client)
    {
        _client = client;
    }

    public IContentfulEntryOperationService Create(IContentfulManagementClientAdapter client)
    {
        CreateWasCalled = true;
        CreateCallCount++;
        
        return new MockEntryOperationService();
    }
}