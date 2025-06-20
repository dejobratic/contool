using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace Contool.Core.Infrastructure.Contentful.Utils;

public class ContentTypeEqualityComparer : IEqualityComparer<ContentType>
{
    private static readonly ContentTypeFieldEqualityComparer FieldComparer = new();

    public bool Equals(ContentType? x, ContentType? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        // Compare top-level properties
        if (!string.Equals(x.Name, y.Name, StringComparison.Ordinal) ||
            !string.Equals(x.Description, y.Description, StringComparison.Ordinal) ||
            !string.Equals(x.DisplayField, y.DisplayField, StringComparison.Ordinal))
            return false;

        // Compare fields count
        if (x.Fields?.Count != y.Fields?.Count)
            return false;

        // Compare fields one by one by Id
        foreach (var xField in x.Fields ?? [])
        {
            var yField = y.Fields?.FirstOrDefault(f => f.Id == xField.Id);
            if (yField is null)
                return false;

            if (!FieldComparer.Equals(xField, yField))
                return false;
        }

        return true;
    }

    public int GetHashCode([DisallowNull] ContentType obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (obj.Name?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.Description?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.DisplayField?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.Fields?.Count ?? 0);
            return hash;
        }
    }
}