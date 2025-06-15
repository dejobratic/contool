using Contentful.Core.Models;
using Contool.Core.Infrastructure.Extensions;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeObject() : ContentFieldType("Object")
{
    public override Type DotnetType => typeof(object);

    public override object? GetValue(object prop, Schema? _)
        => prop.ToString();

    public override object? Deserialize(object? raw, Schema? _)
        => (raw as string)?.DeserializeFromJsonString<JToken>();

    public override bool IsValidRawValue(object? value)
        => value is string || value is JObject;
}
