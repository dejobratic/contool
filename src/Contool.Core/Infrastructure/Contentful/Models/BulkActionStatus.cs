namespace Contool.Core.Infrastructure.Contentful.Models;

public static class BulkActionStatus
{
    public const string Create = "created";
    public const string InProgress = "inProgress";
    public const string Succeeded = "succeeded";
    public const string Failed = "failed";
    
    public static readonly HashSet<string> TerminalStatusCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        Succeeded,
        Failed,
    };
}