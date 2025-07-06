using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysPublishedAtField() : SysField("sys.PublishedAt")
{
    protected override object? Extract(SystemProperties sys)
        => sys.PublishedAt;

    protected override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.PublishedAt = v;
    }
}
