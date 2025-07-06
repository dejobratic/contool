using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysCreatedAtField() : SysField("sys.CreatedAt")
{
    protected override object? Extract(SystemProperties sys)
        => sys.CreatedAt;

    protected override void Apply(SystemProperties sys, object? value)
    {
        if (DateTime.TryParse(value?.ToString(), out var v))
            sys.CreatedAt = v;
    }
}
