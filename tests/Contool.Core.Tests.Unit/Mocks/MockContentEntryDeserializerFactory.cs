using Contool.Core.Features.ContentUpload;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentEntryDeserializerFactory : IContentEntryDeserializerFactory
{
    public bool CreateAsyncWasCalled { get; private set; }
    
    public string? LastContentTypeId { get; private set; }
    
    public IContentfulService? LastContentfulService { get; private set; }
    
    public CancellationToken LastCancellationToken { get; private set; }
    
    
    private IContentEntryDeserializer? _deserializer;

    public void SetupDeserializer(IContentEntryDeserializer deserializer)
    {
        _deserializer = deserializer;
    }

    public Task<IContentEntryDeserializer> CreateAsync(
        string contentTypeId, 
        IContentfulService contentfulService, 
        CancellationToken cancellationToken = default)
    {
        CreateAsyncWasCalled = true;
        LastContentTypeId = contentTypeId;
        LastContentfulService = contentfulService;
        LastCancellationToken = cancellationToken;
        
        return Task.FromResult(_deserializer ?? throw new InvalidOperationException("Deserializer not set up"));
    }
}