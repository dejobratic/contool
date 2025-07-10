using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeBoolean() : ContentFieldType("Boolean")
{
    public override Type DotnetType => typeof(bool);

    public override object GetValue(object prop, Schema? _)
        => Convert.ToBoolean(prop);

    public override object? Deserialize(object? raw, Schema? _)
        => string.IsNullOrWhiteSpace(raw?.ToString()) ? null : Convert.ToBoolean(raw);

    public override bool IsValidRawValue(object? value)
        => bool.TryParse(value?.ToString(), out _);
}
