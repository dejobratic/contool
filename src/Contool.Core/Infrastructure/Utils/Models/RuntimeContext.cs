namespace Contool.Core.Infrastructure.Utils.Models;

public class RuntimeContext : IRuntimeContext
{
    public bool IsDryRun { get; private set; }

    public void SetDryRun(bool isDryRun)
    {
        IsDryRun = isDryRun;
    }
}
