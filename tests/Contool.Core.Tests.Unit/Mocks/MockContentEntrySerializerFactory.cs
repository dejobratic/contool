using Contool.Core.Features.ContentDownload;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentEntrySerializerFactory : IContentEntrySerializerFactory
{
    private IContentEntrySerializer? _serializer;
    private Exception? _exceptionToThrow;

    public bool CreateAsyncWasCalled { get; private set; }
    public string? LastContentTypeId { get; private set; }
    public IContentfulService? LastContentfulService { get; private set; }

    public void SetupSerializer(IContentEntrySerializer serializer)
    {
        _serializer = serializer;
    }

    public void SetupToThrow(Exception exception)
    {
        _exceptionToThrow = exception;
    }

    public async Task<IContentEntrySerializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (_exceptionToThrow != null)
            throw _exceptionToThrow;

        CreateAsyncWasCalled = true;
        LastContentTypeId = contentTypeId;
        LastContentfulService = contentfulService;

        return _serializer ?? throw new InvalidOperationException("Serializer not set up");
    }
}