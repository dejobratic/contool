using Contentful.Core.Models;
using Contool.Core.Contentful.Utils;

namespace Contool.Core.Contentful.Models;

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
