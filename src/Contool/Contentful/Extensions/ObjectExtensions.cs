using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contool.Contentful.Models;
using Contool.Infrastructure.Extensions;
using Html2Markdown;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Dynamic;

namespace Contool.Contentful.Extensions;

internal static class ObjectExtensions
{
    private static readonly JsonSerializerSettings JsonSettings = new() 
    { 
        Converters = [new ContentJsonConverter()],
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    };

    public static object? ToLink(this object value, string? linkType)
    {
        if (value is string linkValue && string.IsNullOrEmpty(linkValue))
        {
            return null;
        }

        var link = new Link(linkType, (string)value);

        var jObj = JObject
            .FromObject(link)
            .ToObject<ExpandoObject>();

        return jObj is null ? null : JObject.FromObject(jObj, JsonSerializer.Create(JsonSettings));
    }

    public static object? ToDocument(this object? value)
    {
        if (value is null)
        {
            return null;
        }

        var doc = new Document
        {
            NodeType = "document",
            Data = new GenericStructureData(),
            Content = [new Paragraph()
            {
                NodeType = "paragraph",
                Data = new GenericStructureData(),
                Content = [new Text()
                {
                    NodeType = "text",
                    Data = new GenericStructureData(),
                    Marks = [],
                    Value = (string)value
                }]
            }]
        };

        var jObj = JObject
            .FromObject(doc)
            .ToObject<ExpandoObject>();

        return jObj is null ? null : JObject.FromObject(jObj, JsonSerializer.Create(JsonSettings));
    }

    public static DateTime? ToDateTime(this object? value)
    {
        if (value is null) return null;

        var date = value.FromInvariantDateTime();

        if (date == DateTime.MinValue) return null;

        // Contentful doesn't support milliseconds although it states it is ISO 8601 compliant :(
        return date.StripMilliseconds();
    }

    private static DateTime StripMilliseconds(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
    }

    public static string? ToArrayString(this object? list, Schema? schema, string arrayDelimiter = "|")
    {
        if (list == null) return null;

        if (schema == null) return null;

        if (list is not JToken jTokenList)
        {
            return null;
        }

        return string.Join(arrayDelimiter, schema.Type == "Link"
            ? jTokenList.Select(t => t["sys"]?["id"]?.ToString())
            : jTokenList.Select(o => o.ToString()));
    }

    public static string? ToMarkDown(this object? richText)
    {
        if (richText == null)
            return null;

        var document = richText.ToString()?.DeserializeFromJsonString<Document>(JsonSettings); ;

        if (document is null)
            return null;

        var htmlRenderer = new HtmlRenderer();

        var html = htmlRenderer.ToHtml(document).Result;

        var converter = new Converter();

        return converter.Convert(html);
    }

    public static JArray? ToObjectArray(this object? value, Schema? schema)
    {
        const string arrayDelimiter = "|";
        const string arrayCfDelimiter = ",";

        if (value is not string stringValue)
            return null;

        var obj = new JArray();

        string[] arr;

        if (stringValue.Contains(arrayDelimiter))
        {
            arr = [.. stringValue.Split(arrayDelimiter).Select(s => s.Trim())];
        }
        else if (stringValue.Contains(arrayCfDelimiter))
        {
            arr = [.. stringValue.Split(arrayCfDelimiter).Select(s => s.Trim())];
        }
        else
        {
            arr = [stringValue];
        }

        foreach (var arrayItem in arr)
        {
            if (schema?.LinkType == "Link")
            {
                var item = ToLink(arrayItem, schema.LinkType);
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
