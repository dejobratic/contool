using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysVersionField() : SysField("sys.Version")
{
    public override object? Extract(SystemProperties sys)
        => sys.Version;

    public override void Apply(SystemProperties sys, object? value)
    {
        _ = int.TryParse(value?.ToString(), out int v);
        sys.Version = v;
    }
}
