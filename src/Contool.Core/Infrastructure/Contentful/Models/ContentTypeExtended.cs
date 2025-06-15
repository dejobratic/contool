using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

public class ContentTypeExtended : ContentType
{
    public int TotalEntries { get; init; }

    public ContentTypeExtended(ContentType contentType, int totalEntries)
    {
        SystemProperties = contentType.SystemProperties;
        Name = contentType.Name;
        Description = contentType.Description;
        DisplayField = contentType.DisplayField;
        Fields = contentType.Fields;
        TotalEntries = totalEntries;
    }
}
