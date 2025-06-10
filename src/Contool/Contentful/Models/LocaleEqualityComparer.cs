using Contentful.Core.Models.Management;
using System.Diagnostics.CodeAnalysis;

namespace Contool.Contentful.Models;

public class LocaleEqualityComparer : IEqualityComparer<Locale>
{
    public bool Equals(Locale? x, Locale? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return string.Equals(x.Code, y.Code, StringComparison.Ordinal)
            && string.Equals(x.Name, y.Name, StringComparison.Ordinal)
            && x.FallbackCode == y.FallbackCode
            && x.Default == y.Default
            && x.Optional == y.Optional;
    }

    public int GetHashCode([DisallowNull] Locale obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (obj.Code?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.Name?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.FallbackCode?.GetHashCode() ?? 0);
            hash = hash * 23 + obj.Default.GetHashCode();
            hash = hash * 23 + obj.Optional.GetHashCode();
            return hash;
        }
    }
}