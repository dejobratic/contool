namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentfulManagementClientFactory
{
    public bool CreateWasCalled { get; private set; }
    public int CreateCallCount { get; private set; }
    public string? LastSpaceId { get; private set; }
    public string? LastEnvironmentId { get; private set; }

    private MockContentfulManagementClient? _client;

    public void SetupClient(MockContentfulManagementClient client)
    {
        _client = client;
    }

    public MockContentfulManagementClient Create(string spaceId, string environmentId)
    {
        CreateWasCalled = true;
        CreateCallCount++;
        LastSpaceId = spaceId;
        LastEnvironmentId = environmentId;
        return _client ?? new MockContentfulManagementClient();
    }
}