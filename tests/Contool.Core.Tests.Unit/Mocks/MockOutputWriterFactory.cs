using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockOutputWriterFactory : IOutputWriterFactory
{
    private IOutputWriter? _outputWriter;

    public bool CreateWasCalled { get; private set; }
    public DataSource? LastDataSource { get; private set; }

    public void SetupOutputWriter(IOutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    public IOutputWriter Create(DataSource dataSource)
    {
        CreateWasCalled = true;
        LastDataSource = dataSource;
        
        return _outputWriter ?? throw new InvalidOperationException("Output writer not set up");
    }
}