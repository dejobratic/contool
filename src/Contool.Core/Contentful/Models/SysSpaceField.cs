using Contentful.Core.Models;

namespace Contool.Core.Contentful.Models;

internal class SysSpaceField() : SysField("sys.Space")
{
    public override object? Extract(SystemProperties sys)
        => sys.Space?.SystemProperties.Id;

    public override void Apply(SystemProperties sys, object? value)
        => sys.Space.SystemProperties.Id = value?.ToString();
}
