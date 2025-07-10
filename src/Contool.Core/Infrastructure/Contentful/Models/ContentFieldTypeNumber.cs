using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeNumber() : ContentFieldType("Number")
{
    public override Type DotnetType => typeof(double);

    public override object GetValue(object prop, Schema? _)
        => Convert.ToDouble(prop);

    public override object Deserialize(object? raw, Schema? _)
        => Convert.ToDouble(raw is string s && string.IsNullOrWhiteSpace(s) ? null : raw);

    public override bool IsValidRawValue(object? value)
        => double.TryParse(value?.ToString(), out _);
}
