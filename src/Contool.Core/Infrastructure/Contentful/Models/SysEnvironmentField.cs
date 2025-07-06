using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysEnvironmentField() : SysField("sys.Environment")
{
    protected override object? Extract(SystemProperties sys)
        => sys.Environment?.SystemProperties.Id;

    protected override void Apply(SystemProperties sys, object? value)
        => sys.Environment.SystemProperties.Id = value?.ToString();
}
