﻿using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Models;
using Newtonsoft.Json.Linq;

namespace Contool.Core.Features.ContentUpload;

public class ContentEntryDeserializer(ContentType contentType, ContentLocales locales) : IContentEntryDeserializer
{
    private readonly Dictionary<string, ContentField> _fields = contentType.Fields
        .Select(field => new ContentField(field, locales))
        .SelectMany(field => field.FieldNames.Select(name => (name, field)))
        .ToDictionary(tuple => tuple.name, tuple => tuple.field);

    public Entry<dynamic> Deserialize(dynamic row)
    {
        var entry = CreateEntry();

        var data = row as IDictionary<string, object?>
            ?? throw new ArgumentException("Row must be a dictionary.");

        foreach (var heading in data.Keys)
        {
            if (SysField.Names.Contains(heading))
            {
                SysField.Deserialize(entry.SystemProperties, heading, data[heading]);
            }
            else if (_fields.TryGetValue(heading, out var field))
            {
                var locale = new ContentFieldName(heading).Locale 
                    ?? locales.DefaultLocale;

                field.Deserialize(entry.Fields, locale, data[heading]);
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

