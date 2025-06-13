using Contentful.Core.Models;

namespace Contool.Core.Contentful.Models;

internal class SysEnvironmentField() : SysField("sys.Environment")
{
    public override object? Extract(SystemProperties sys)
        => sys.Environment?.SystemProperties.Id;

    public override void Apply(SystemProperties sys, object? value)
        => sys.Environment.SystemProperties.Id = value?.ToString();
}
