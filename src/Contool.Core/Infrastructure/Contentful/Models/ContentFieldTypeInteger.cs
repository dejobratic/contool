using Contentful.Core.Models;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeInteger() : ContentFieldType("Integer")
{
    public override Type DotnetType => typeof(long);

    public override object GetValue(object prop, Schema? _)
        => Convert.ToInt64(prop);

    public override object Deserialize(object? raw, Schema? _)
        => Convert.ToInt64(raw is string s && string.IsNullOrWhiteSpace(s) ? null : raw);

    public override bool IsValidRawValue(object? value)
    {
        return value switch
        {
            null => true,
            string s when string.IsNullOrWhiteSpace(s) => true,
            long => true, // already a long
            int => true, // convertible without parsing
            string s => long.TryParse(s, out _),
            JObject jObj => TryParseFirstPropertyValue(jObj),
            JToken jToken => TryParseToken(jToken),
            _ => long.TryParse(value?.ToString(), out _)
        };

        static bool TryParseFirstPropertyValue(JObject jObj)
        {
            var firstValue = jObj.Properties().FirstOrDefault()?.Value;
            return firstValue is not null && long.TryParse(firstValue.ToString(), out _);
        }

        static bool TryParseToken(JToken token)
        {
            // Avoid ToString if possible
            return token.Type switch
            {
                JTokenType.Integer => true,
                JTokenType.String => long.TryParse(token.Value<string>(), out _),
                _ => long.TryParse(token.ToString(), out _)
            };
        }
    }
}
