using Contool.Contentful.Models;
using Contentful.Core.Models;

namespace Contool.Contentful.Services;

public class ContentEntrySerializer(ContentType contentType, ContentLocales locales) : IContentEntrySerializer
{
    private readonly List<ContentField> _fields = [.. contentType.Fields.Select(f => new ContentField(f, locales))];

    public string[] FieldNames => [.. SystemField.Names, .. _fields.SelectMany(f => f.FieldNames)];

    public dynamic Serialize(Entry<dynamic> entry)
    {
        var result = new Dictionary<string, object?>();
        SystemField.Serialize(entry.SystemProperties, result);

        foreach (var field in _fields)
        {
            foreach (var locale in field.Locales)
            {
                var fieldName = new ContentFieldName(field.Id, field.Type, locale).Value;
                result[fieldName] = field.Serialize(entry.Fields, locale);
            }
        }
        return result;
    }
}