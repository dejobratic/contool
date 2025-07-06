using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysUpdatedAtField() : SysField("sys.UpdatedAt")
{
    protected override object? Extract(SystemProperties sys)
        => sys.UpdatedAt;

    protected override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.UpdatedAt = v;
    }
}
