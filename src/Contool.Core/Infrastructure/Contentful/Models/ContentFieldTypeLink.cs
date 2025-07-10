using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeLink() : ContentFieldType("Link")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object prop, Schema? _)
        => (prop as JObject)?["sys"]?["id"]?.ToString();

    public override object? Deserialize(object? raw, Schema? schema)
        => raw?.ToLink(schema?.LinkType);

    public override bool IsValidRawValue(object? value)
        => value is string or JObject;
}
