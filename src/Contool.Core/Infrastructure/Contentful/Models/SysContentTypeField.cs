using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class SysContentTypeField() : SysField("sys.ContentType")
{
    protected override object? Extract(SystemProperties sys)
        => sys.ContentType?.SystemProperties.Id;

    protected override void Apply(SystemProperties sys, object? value)
        => sys.ContentType.SystemProperties.Id = value?.ToString();
}
