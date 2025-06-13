using Contentful.Core.Models;
using Contool.Core.Contentful.Extensions;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Contentful.Models;

internal class ContentFieldTypeArray() : ContentFieldType("Array")
{
    private const string ArrayDelimiter = "|";
    private const string ArrayCfDelimiter = ",";

    public override Type DotnetType => typeof(IEnumerable<object>);

    public override object? GetValue(object prop, Schema? schema)
        => ToArrayString(prop, schema);

    public override object? Deserialize(object? raw, Schema? schema)
        => ToObjectArray(raw, schema);

    public override bool IsValidRawValue(object? value)
        => value is string || value is IEnumerable<object>;

    private static string? ToArrayString(object? list, Schema? schema)
    {
        if (list == null) return null;

        if (schema == null) return null;

        if (list is not JToken jTokenList)
        {
            return null;
        }

        return string.Join(ArrayDelimiter, schema.Type == "Link"
            ? jTokenList.Select(t => t["sys"]?["id"]?.ToString())
            : jTokenList.Select(o => o.ToString()));
    }

    private static JArray? ToObjectArray(object? value, Schema? schema)
    {
        if (value is not string stringValue)
            return null;

        var obj = new JArray();

        string[] arr;

        if (stringValue.Contains(ArrayDelimiter))
        {
            arr = [.. stringValue.Split(ArrayDelimiter).Select(s => s.Trim())];
        }
        else if (stringValue.Contains(ArrayCfDelimiter))
        {
            arr = [.. stringValue.Split(ArrayCfDelimiter).Select(s => s.Trim())];
        }
        else
        {
            arr = [stringValue];
        }

        foreach (var arrayItem in arr)
        {
            if (schema?.LinkType == "Link")
            {
                var item = arrayItem.ToLink(schema.LinkType);
                if (item is not null)
                {
                    obj.Add(item);
                }
            }
            else
            {
                obj.Add(arrayItem);
            }
        }

        return obj;
    }
}
