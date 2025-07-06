using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysPublishedVersionField() : SysField("sys.PublishedVersion")
{
    protected override object? Extract(SystemProperties sys)
        => sys.PublishedVersion;

    protected override void Apply(SystemProperties sys, object? value)
    {
        if (int.TryParse(value?.ToString(), out var v))
            sys.PublishedVersion = v;
    }
}
