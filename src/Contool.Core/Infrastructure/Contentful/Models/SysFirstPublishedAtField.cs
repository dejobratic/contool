using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysFirstPublishedAtField() : SysField("sys.FirstPublishedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.FirstPublishedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.FirstPublishedAt = v;
    }
}
