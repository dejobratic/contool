using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockInputReaderFactory : IInputReaderFactory
{
    public bool CreateWasCalled { get; private set; }
    public DataSource? LastDataSource { get; private set; }
    
    private IInputReader? _reader;

    public void SetupReader(IInputReader reader)
    {
        _reader = reader;
    }

    public IInputReader Create(DataSource dataSource)
    {
        CreateWasCalled = true;
        LastDataSource = dataSource;
        return _reader ?? throw new InvalidOperationException("Reader not set up");
    }
}