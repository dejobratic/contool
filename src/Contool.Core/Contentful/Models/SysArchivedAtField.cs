using Contentful.Core.Models;

namespace Contool.Core.Contentful.Models;

internal class SysArchivedAtField() : SysField("sys.ArchivedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.ArchivedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.ArchivedAt = v;
    }
}
