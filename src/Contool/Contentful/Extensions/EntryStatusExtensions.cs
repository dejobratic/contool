using Contentful.Core.Models;
using Newtonsoft.Json.Linq;

namespace Contool.Contentful.Extensions;

// https://www.contentful.com/developers/docs/tutorials/general/determine-entry-asset-state/
public static class EntryStatusExtensions
{
    public static bool IsDraft(this Entry<dynamic> entry)
    {
        return entry.SystemProperties.PublishedVersion is null or 0;
    }

    public static bool IsChanged(this Entry<dynamic> entry)
    {
        return entry.SystemProperties.PublishedVersion is not null
            && entry.SystemProperties.Version >= entry.SystemProperties.PublishedVersion + 2;
    }

    public static bool IsPublished(this Entry<dynamic> entry)
    {
        return entry.SystemProperties.PublishedVersion is not null
            && entry.SystemProperties.Version == entry.SystemProperties.PublishedVersion + 1;
    }

    public static bool IsArchived(this Entry<dynamic> entry)
    {
        return entry.SystemProperties.ArchivedVersion is not null;
    }
}
