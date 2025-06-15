using Contentful.Core.Models;
using Contool.Core.Infrastructure.Extensions;
using Html2Markdown;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeRichText() : ContentFieldType("RichText")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object prop, Schema? _)
        => ToMarkDown(prop);

    public override object? Deserialize(object? raw, Schema? _)
        => ToDocument(raw);

    public override bool IsValidRawValue(object? value)
        => value is string || value is JObject;

    public static object? ToDocument(object? value)
    {
        if (value is null)
            return null;

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

        return doc.SerializeToJsonObject();
    }

    public static string? ToMarkDown(object? richText)
    {
        if (richText == null)
            return null;

        var document = richText
            .ToString()
            ?.DeserializeFromJsonString<Document>(); ;

        if (document is null)
            return null;

        var htmlRenderer = new HtmlRenderer();

        var html = htmlRenderer.ToHtml(document).Result;

        var converter = new Converter();

        return converter.Convert(html);
    }

}
