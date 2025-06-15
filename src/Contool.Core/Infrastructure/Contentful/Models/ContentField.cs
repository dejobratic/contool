using Contentful.Core.Models;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Infrastructure.Contentful.Models;

public class ContentField(Field field, ContentLocales locales)
{
    public string Id { get; } = field.Id;

    public ContentFieldType Type { get; } = ContentFieldType.FromName(field.Type);

    public string[] Locales { get; } = field.Localized ? [.. locales] : [locales.DefaultLocale];

    private Schema? Schema { get; } = field.Items;

    public IEnumerable<string> FieldNames =>
        Locales.Select(locale => new ContentFieldName(Id, Type, locale).Value);

    public object? Serialize(JObject fields, string localeCode)
    {
        if (fields.TryGetValue(Id, out var value)
            && value is JObject localizedObj
            && localizedObj.TryGetValue(localeCode, out var token))
            return Type.GetValue(token, Schema);

        return null;
    }

    public void Deserialize(JObject fields, string localeCode, object? rawValue)
    {
        if (!Type.IsValidRawValue(rawValue))
            return;

        var token = Type.Deserialize(rawValue, Schema);

        if (token is null)
            return;

        var container = fields[Id] as JObject ?? [];
        container[localeCode] = token is JToken jt ? jt : JToken.FromObject(token!);
        fields[Id] = container;
    }
}
