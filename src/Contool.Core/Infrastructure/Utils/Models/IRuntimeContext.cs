namespace Contool.Core.Infrastructure.Utils.Models;

public interface IRuntimeContext
{
    public bool IsDryRun { get; }

    void SetDryRun(bool isDryRun);
}
