using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysSpaceField() : SysField("sys.Space")
{
    protected override object? Extract(SystemProperties sys)
        => sys.Space?.SystemProperties.Id;

    protected override void Apply(SystemProperties sys, object? value)
        => sys.Space.SystemProperties.Id = value?.ToString();
}
