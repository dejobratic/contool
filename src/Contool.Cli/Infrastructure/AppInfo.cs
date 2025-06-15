using Contool.Cli.Infrastructure.Utils;

namespace Contool.Cli.Infrastructure;

public static class AppInfo
{
    public const string Name = "contool";
    public const string LongName = "Contentful Update Tool";
    public const string Description = "Bulk import/export/update and delete Contentful content from and to EXCEL/CSV/JSON";
    public const string SourceRepo = "https://github.com/dejobratic/contool";
    public const string ReleasePage = $"https://github.com/dejobratic/contool/releases/latest";
    public static readonly string VersionVersion = VersionChecker.GetInstalledCliVersion();
}
