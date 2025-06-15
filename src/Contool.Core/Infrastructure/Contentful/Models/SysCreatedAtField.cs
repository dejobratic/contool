using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysCreatedAtField() : SysField("sys.CreatedAt")
{
    public override object? Extract(SystemProperties sys)
        => sys.CreatedAt;

    public override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.CreatedAt = v;
    }
}
