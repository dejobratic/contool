using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockRuntimeContext : IRuntimeContext
{
    public bool IsDryRun { get; private set; }
    
    public void SetDryRun(bool isDryRun)
    {
        IsDryRun = isDryRun;
    }
}