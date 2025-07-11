namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentfulClientFactory
{
    public bool CreateWasCalled { get; private set; }
    public int CreateCallCount { get; private set; }
    public string? LastSpaceId { get; private set; }
    public string? LastEnvironmentId { get; private set; }

    private MockContentfulClient? _client;

    public void SetupClient(MockContentfulClient client)
    {
        _client = client;
    }

    public MockContentfulClient Create(string spaceId, string environmentId)
    {
        CreateWasCalled = true;
        CreateCallCount++;
        LastSpaceId = spaceId;
        LastEnvironmentId = environmentId;
        return _client ?? new MockContentfulClient();
    }
}