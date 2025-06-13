using Contentful.Core.Models;

namespace Contool.Core.Contentful.Models;

internal class SysUpdatedAtField() : SysField("sys.UpdatedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.UpdatedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.UpdatedAt = v;
    }
}
