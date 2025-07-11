using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeText() : ContentFieldType("Text")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object? prop, Schema? _)
        => prop?.ToString();

    public override object? Deserialize(object? raw, Schema? _)
        => raw?.ToString();

    public override bool IsValidRawValue(object? value)
        => value is string || (value != null && value.ToString() is not null);
}
