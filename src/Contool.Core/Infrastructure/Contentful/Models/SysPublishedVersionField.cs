using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysPublishedVersionField() : SysField("sys.PublishedVersion")
{
    public override object? Extract(SystemProperties sys)
        => sys.PublishedVersion;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (int.TryParse(value?.ToString(), out var v))
            sys.PublishedVersion = v;
    }
}
