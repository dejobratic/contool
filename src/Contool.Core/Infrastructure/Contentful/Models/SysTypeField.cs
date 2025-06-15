using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysTypeField() : SysField("sys.Type")
{
    public override object? Extract(SystemProperties sys)
        => sys.Type;

    public override void Apply(SystemProperties sys, object? value)
        => sys.Type = value?.ToString();
}
