using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Contentful.Models;
using Newtonsoft.Json.Linq;

namespace Contool.Features.EntryUpload;

internal class ContentEntryDeserializer(ContentType contentType, ContentLocales locales) : IContentEntryDeserializer
{
    private readonly Dictionary<string, ContentField> _fields = contentType.Fields
        .Select(f => new ContentField(f, locales))
        .SelectMany(f => f.FieldNames.Select(name => (name, field: f)))
        .ToDictionary(t => t.name, t => t.field);

    public Entry<dynamic> Deserialize(ContentFieldName[] headings, dynamic row)
    {
        var entry = CreateEntry();

        var data = row as IDictionary<string, object?>
            ?? throw new ArgumentException("Row must be a dictionary.");

        foreach (var heading in headings)
        {
            if (SysField.Names.Contains(heading.Value))
            {
                SysField.Deserialize(entry.SystemProperties, heading.Value, data[heading.Value]);
            }
            else if (_fields.TryGetValue(heading.Value, out var field))
            {
                field.Deserialize(entry.Fields, heading.Locale, data[heading.Value]);
            }
        }

        return entry;
    }

    private static Entry<dynamic> CreateEntry()
    {
        return new Entry<dynamic>
        {
            SystemProperties = new SystemProperties
            {
                ContentType = new ContentType
                {
                    SystemProperties = new SystemProperties()
                },
                Space = new Space
                {
                    SystemProperties = new SystemProperties()
                },
                Environment = new ContentfulEnvironment
                {
                    SystemProperties = new SystemProperties()
                },
            },
            Metadata = new ContentfulMetadata
            {
                Tags = [],
                Concepts = [],
            },
            Fields = new JObject(),
        };
    }
}

