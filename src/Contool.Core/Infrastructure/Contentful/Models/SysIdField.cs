using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Utils;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysIdField() : SysField("sys.Id")
{
    public override object? Extract(SystemProperties sys)
        => sys.Id;

    public override void Apply(SystemProperties sys, object? value)
    {
        sys.Id = value?.ToString();

        if (string.IsNullOrEmpty(sys.Id))
            sys.Id = ContentfulIdGenerator.NewId();
    }
}
