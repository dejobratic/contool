using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Utils;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysIdField() : SysField("sys.Id")
{
    protected override object? Extract(SystemProperties sys)
        => sys.Id;

    protected override void Apply(SystemProperties sys, object? value)
    {
        sys.Id = value?.ToString();

        if (string.IsNullOrEmpty(sys.Id))
            sys.Id = ContentfulIdGenerator.NewId();
    }
}
