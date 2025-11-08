using Contentful.Core.Models;
using Contool.Core.Infrastructure.Extensions;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeRichText() : ContentFieldType("RichText")
{
    public override Type DotnetType => typeof(string);

    public override object? GetValue(object prop, Schema? _)
        => ToPlainText(prop);

    public override object? Deserialize(object? raw, Schema? _)
        => ToDocument(raw);

    public override bool IsValidRawValue(object? value)
        => value is string or JObject || (value?.ToString() != null);

    private static JObject? ToDocument(object? value)
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

    private static string? ToPlainText(object? richText)
    {
        var document = richText
            ?.ToString()
            ?.DeserializeFromJsonString<Document>();

        if (document is null)
            return null;

        var builder = new StringBuilder();
        ExtractTextFromContent(document.Content, builder);
        
        return builder.ToString();
    }

    private static void ExtractTextFromContent(List<IContent>? content, StringBuilder builder)
    {
        if (content == null)
            return;

        foreach (var node in content)
        {
            switch (node)
            {
                case Text textNode:
                    builder.Append(textNode.Value);
                    break;
                case Paragraph paragraph:
                    ExtractTextFromParagraph(paragraph, builder);
                    break;
                default:
                    ExtractTextFromNode(node, builder);
                    break;
            }
        }
    }

    private static void ExtractTextFromParagraph(Paragraph paragraph, StringBuilder builder)
    {
        ExtractTextFromContent(paragraph.Content, builder);
        builder.AppendLine();
    }

    private static void ExtractTextFromNode(IContent node, StringBuilder builder)
    {
        if (node.GetType().GetProperty("Content")?.GetValue(node) is not List<IContent> content)
            return;
        
        ExtractTextFromContent(content, builder);
        
        if (IsBlockLevelElement(node))
            builder.AppendLine();
    }

    private static bool IsBlockLevelElement(IContent node)
    {
        var nodeType = node.GetType().Name;
        return nodeType is "Paragraph" or "Heading" or "ListItem" or "Quote" or "BlockQuote";
    }
}
