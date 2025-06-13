using Contentful.Core.Models;

namespace Contool.Core.Contentful.Models;

internal class SysArchivedVersionField() : SysField("sys.ArchivedVersion")
{
    public override object? Extract(SystemProperties sys)
        => sys.ArchivedVersion;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (int.TryParse(value?.ToString(), out var v))
            sys.ArchivedVersion = v;
    }
}
