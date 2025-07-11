using Contool.Core.Features.ContentDelete;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentDeleter : IContentDeleter
{
    public bool DeleteAsyncWasCalled { get; private set; }
    
    public ContentDeleterInput? LastInput { get; private set; }
    
    public CancellationToken LastCancellationToken { get; private set; }
    
    private Exception? _exceptionToThrow;

    public async Task DeleteAsync(ContentDeleterInput input, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (_exceptionToThrow != null)
            throw _exceptionToThrow;

        DeleteAsyncWasCalled = true;
        LastInput = input;
        LastCancellationToken = cancellationToken;
        
        await Task.CompletedTask;
    }

    public void SetupToThrow(Exception exception)
    {
        _exceptionToThrow = exception;
    }
}