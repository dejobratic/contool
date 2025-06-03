using Contool.Contentful.Models;
using Contool.Extensions;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Html2Markdown;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Dynamic;

namespace Contool.Contentful.Extensions;

internal static class ObjectExtensions
{
    private static readonly JsonSerializerSettings _jsonSettings = new() { Converters = [new ContentJsonConverter()] };

    public static object? ToLink(this object value, string linkType)
    {
        if (value is string linkValue && string.IsNullOrEmpty(linkValue))
        {
            return null;
        }

        var link = new Link(linkType, (string)value);

        var jObj = JObject
            .FromObject(link)
            .ToObject<ExpandoObject>();

        if (jObj is null) return null;

        return JObject.FromObject(jObj, JsonSerializer.Create(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
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
            Data = new(),
            Content = [new Paragraph()
            {
                NodeType = "paragraph",
                Data = new(),
                Content = [new Text()
                {
                    NodeType = "text",
                    Data = new(),
                    Marks = [],
                    Value = (string)value
                }]
            }]
        };

        var jObj = JObject
            .FromObject(doc)
            .ToObject<ExpandoObject>();

        if (jObj is null) return null;

        return JObject.FromObject(jObj, JsonSerializer.Create(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }

    public static DateTime? ToDateTime(this object? value)
    {
        if (value is null) return null;

        var date = value.FromInvariantDateTime();

        if (date == DateTime.MinValue) return null;

        // Contentful doesn't support milliseconds although it states it is ISO 8601 compliant :(
        return date.StripMilliseconds();
    }

    public static DateTime StripMilliseconds(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
    }

    public static string? ToArrayString(this object? list, Schema schema, string arrayDelimiter = "|")
    {
        if (list == null) return null;

        if (schema == null) return null;

        if (list is not JToken jTokenList)
        {
            return null;
        }

        if (schema.Type == "Link")
        {
            return string.Join(arrayDelimiter, jTokenList.Select(t => t["sys"]?["id"]?.ToString()));
        }

        return string.Join(arrayDelimiter, jTokenList.Select(o => o.ToString()));
    }

    public static string? ToMarkDown(this object? richText)
    {
        if (richText == null) return null;

        var document = richText.ToString()?.DeserializeFromJsonString<Document>(_jsonSettings); ;

        if (document is null) return null;

        var _htmlRenderer = new HtmlRenderer();

        var html = _htmlRenderer.ToHtml(document).Result;

        var converter = new Converter();

        return converter.Convert(html);
    }

    public static JArray? ToObjectArray(this object? value, Schema schema)
    {
        var arrayDelimiter = "|";
        var arrayCfDelimiter = ",";

        if (value is string stringValue)
        {
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
                if (schema.LinkType == "Link")
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
        return null;
    }
}
