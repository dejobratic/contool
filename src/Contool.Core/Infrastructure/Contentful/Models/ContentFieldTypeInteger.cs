using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeInteger() : ContentFieldType("Integer")
{
    public override Type DotnetType => typeof(long);

    public override object? GetValue(object prop, Schema? _)
        => Convert.ToInt64(prop);

    public override object? Deserialize(object? raw, Schema? _)
        => Convert.ToInt64(raw is string s && string.IsNullOrWhiteSpace(s) ? null : raw);

    public override bool IsValidRawValue(object? value)
        => long.TryParse(value?.ToString(), out _);
}
