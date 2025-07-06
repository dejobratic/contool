using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysArchivedAtField() : SysField("sys.ArchivedAt")
{
    protected override object? Extract(SystemProperties sys)
        => sys.ArchivedAt;

    protected override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.ArchivedAt = v;
    }
}
