using Contentful.Core.Models.Management;
using Contool.Core.Contentful.Utils;

namespace Contool.Core.Contentful.Extensions;

public static class LocaleExtensions
{
    private static readonly LocaleEqualityComparer LocaleComparer = new();

    public static bool IsEquivalentTo(this IEnumerable<Locale> source, IEnumerable<Locale> target)
    {
        if (ReferenceEquals(source, target))
            return true;

        if (source is null || target is null)
            return false;

        var sourceSet = new HashSet<Locale>(source, LocaleComparer);
        var targetSet = new HashSet<Locale>(target, LocaleComparer);

        return sourceSet.SetEquals(targetSet);
    }
}