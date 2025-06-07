using Contool.Contentful.Extensions;
using Contool.Extensions;
using Contentful.Core.Models;
using Newtonsoft.Json.Linq;

namespace Contool.Contentful.Models;

public abstract class FieldType(string name)
{
    public string Name { get; } = name;

    public abstract Type DotnetType { get; }

    public abstract object? GetValue(object prop, Schema? schema);

    public abstract object? Deserialize(object? raw, Schema? schema);

    public abstract bool IsValidRawValue(object? value);

    public override string ToString() => Name;

    private static IEnumerable<FieldType> All =>
    [
        new SymbolFieldType(),
        new TextFieldType(),
        new RichTextFieldType(),
        new IntegerFieldType(),
        new NumberFieldType(),
        new DateFieldType(),
        new BooleanFieldType(),
        new LinkFieldType(),
        new ArrayFieldType(),
        new ObjectFieldType(),
    ];

    public static FieldType FromName(string name)
    {
        return All.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Unsupported FieldType: {name}");
    }
}

class SymbolFieldType() : FieldType("Symbol")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object prop, Schema? _) => prop?.ToString();

    public override object? Deserialize(object? raw, Schema? _) => raw?.ToString();

    public override bool IsValidRawValue(object? value) => value is string;
}

class TextFieldType() : FieldType("Text")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object prop, Schema? _) => prop?.ToString();

    public override object? Deserialize(object? raw, Schema? _) => raw?.ToString();

    public override bool IsValidRawValue(object? value) => value is string;
}

class RichTextFieldType() : FieldType("RichText")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object prop, Schema? _) => prop.ToMarkDown();

    public override object? Deserialize(object? raw, Schema? _) => raw?.ToDocument();

    public override bool IsValidRawValue(object? value) => value is string || value is JObject;
}

class IntegerFieldType() : FieldType("Integer")
{
    public override Type DotnetType => typeof(long);

    public override object? GetValue(object prop, Schema? _) => Convert.ToInt64(prop);

    public override object? Deserialize(object? raw, Schema? _) => Convert.ToInt64(raw is string s && string.IsNullOrWhiteSpace(s) ? null : raw);

    public override bool IsValidRawValue(object? value) => long.TryParse(value?.ToString(), out _);
}

class NumberFieldType() : FieldType("Number")
{
    public override Type DotnetType => typeof(double);

    public override object? GetValue(object prop, Schema? _) => Convert.ToDouble(prop);

    public override object? Deserialize(object? raw, Schema? _) => Convert.ToDouble(raw is string s && string.IsNullOrWhiteSpace(s) ? null : raw);

    public override bool IsValidRawValue(object? value) => double.TryParse(value?.ToString(), out _);
}

class DateFieldType() : FieldType("Date")
{
    public override Type DotnetType => typeof(DateTime);

    public override object? GetValue(object prop, Schema? _) => DateTime.Parse(prop.ToString()!);

    public override object? Deserialize(object? raw, Schema? _) => raw?.ToDateTime();

    public override bool IsValidRawValue(object? value) => DateTime.TryParse(value?.ToString(), out _);
}

class BooleanFieldType() : FieldType("Boolean")
{
    public override Type DotnetType => typeof(bool);

    public override object? GetValue(object prop, Schema? _) => Convert.ToBoolean(prop);

    public override object? Deserialize(object? raw, Schema? _) => string.IsNullOrWhiteSpace(raw?.ToString()) ? null : Convert.ToBoolean(raw);

    public override bool IsValidRawValue(object? value) => bool.TryParse(value?.ToString(), out _);
}

class LinkFieldType() : FieldType("Link")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object prop, Schema? _) => (prop as JObject)?["sys"]?["id"]?.ToString();

    public override object? Deserialize(object? raw, Schema? schema) => raw?.ToLink(schema.LinkType);

    public override bool IsValidRawValue(object? value) => value is string || value is JObject;
}

class ArrayFieldType() : FieldType("Array")
{
    public override Type DotnetType => typeof(IEnumerable<object>);

    public override object? GetValue(object prop, Schema? schema) => (prop as JToken)?.ToArrayString(schema);

    public override object? Deserialize(object? raw, Schema? schema) => raw?.ToObjectArray(schema);

    public override bool IsValidRawValue(object? value) => value is string || value is IEnumerable<object>;
}

class ObjectFieldType() : FieldType("Object")
{
    public override Type DotnetType => typeof(object);

    public override object? GetValue(object prop, Schema? _) => prop.ToString();

    public override object? Deserialize(object? raw, Schema? _) => (raw as string)?.DeserializeFromJsonString<JToken>();

    public override bool IsValidRawValue(object? value) => value is string || value is JObject;
}
