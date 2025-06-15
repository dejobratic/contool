using Contentful.Core.Models;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Infrastructure.Contentful.Extensions;

public static class EntryReferenceExtensions
{
    public static IEnumerable<string> GetReferencedEntryIds(this IEnumerable<Entry<dynamic>> entries)
        => entries
        .SelectMany(e => e.GetReferencedEntryIds())
        .Distinct();

    public static IEnumerable<string> GetReferencedEntryIds(this Entry<dynamic> entry)
    {
        var referencedIds = new HashSet<string>();

        foreach (var field in entry.Fields)
        {
            if (field.Value is JObject localizedObject)
            {
                foreach (var localeProperty in localizedObject.Properties())
                {
                    var localeValue = localeProperty.Value;

                    ExtractReferences(referencedIds, localeValue);
                }
            }
        }

        return referencedIds;
    }

    private static void ExtractReferences(HashSet<string> referencedIds, JToken localeValue)
    {
        switch (localeValue.Type)
        {
            case JTokenType.Object:
                ExtractReference(referencedIds, (JObject)localeValue);
                break;

            case JTokenType.Array:
                foreach (var item in (JArray)localeValue)
                {
                    if (item is not JObject itemObj)
                        continue;

                    ExtractReference(referencedIds, itemObj);
                }
                break;

            // Other types → ignore (string, number, etc.)
            default:
                break;
        }
    }

    private static void ExtractReference(HashSet<string> referencedIds, JObject obj)
    {
        var sys = obj["sys"];
        if (sys == null) return;

        var linkType = (string?)sys["linkType"];
        var id = (string?)sys["id"];

        if (linkType == "Entry" && !string.IsNullOrEmpty(id))
        {
            referencedIds.Add(id);
        }
    }
}
