using Contentful.Core.Models;

namespace Contool.Contentful.Extensions;

// https://www.contentful.com/developers/docs/tutorials/general/determine-entry-asset-state/
public static class EntryExtensions
{
    public static bool IsDraft<T>(this Entry<T> entry)
    {
        return entry.SystemProperties.PublishedVersion is null 
            || entry.SystemProperties.PublishedVersion == 0;
    }

    public static bool IsChanged<T>(this Entry<T> entry)
    {
        return entry.SystemProperties.PublishedVersion is not null 
            && entry.SystemProperties.Version >= entry.SystemProperties.PublishedVersion + 2;
    }

    public static bool IsPublished<T>(this Entry<T> entry)
    {
        return entry.SystemProperties.PublishedVersion is not null 
            && entry.SystemProperties.Version == entry.SystemProperties.PublishedVersion + 1;
    }

    public static bool IsArchived<T>(this Entry<T> entry)
    {
        return entry.SystemProperties.ArchivedVersion is not null;
    }
}
