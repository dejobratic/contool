using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysArchivedVersionField() : SysField("sys.ArchivedVersion")
{
    protected override object? Extract(SystemProperties sys)
        => sys.ArchivedVersion;

    protected override void Apply(SystemProperties sys, object? value)
    {
        if (int.TryParse(value?.ToString(), out var v))
            sys.ArchivedVersion = v;
    }
}
