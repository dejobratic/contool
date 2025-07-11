using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockOutputWriter : IOutputWriter
{
    private Exception? _exceptionToThrow;

    public bool SaveAsyncWasCalled { get; private set; }
    public string? LastSavedPath { get; private set; }
    public IAsyncEnumerable<dynamic>? LastSavedContent { get; private set; }

    public DataSource DataSource { get; } = DataSource.Csv;

    public void SetupToThrow(Exception exception)
    {
        _exceptionToThrow = exception;
    }

    public async Task SaveAsync(string path, IAsyncEnumerable<dynamic> content, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (_exceptionToThrow != null)
            throw _exceptionToThrow;

        SaveAsyncWasCalled = true;
        LastSavedPath = path;
        LastSavedContent = content;

        // Simulate consuming the content
        await foreach (var item in content)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}