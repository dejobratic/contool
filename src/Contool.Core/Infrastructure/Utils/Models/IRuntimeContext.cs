namespace Contool.Core.Infrastructure.Utils.Models;

public interface IRuntimeContext
{
    // TODO: currently don't know a better way to handle dry run state
    public bool IsDryRun { get; }

    void SetDryRun(bool isDryRun);
}
