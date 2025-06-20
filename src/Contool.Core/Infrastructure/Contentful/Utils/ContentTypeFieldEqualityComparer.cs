using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System.Diagnostics.CodeAnalysis;

namespace Contool.Core.Infrastructure.Contentful.Utils;

public class ContentTypeFieldEqualityComparer : IEqualityComparer<Field>
{
    public static readonly ContentTypeFieldValidatorEqualityComparer ValidatorComparer = new();

    public bool Equals(Field? x, Field? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return string.Equals(x.Name, y.Name, StringComparison.Ordinal) &&
               string.Equals(x.Type, y.Type, StringComparison.Ordinal) &&
               string.Equals(x.LinkType, y.LinkType, StringComparison.Ordinal) &&
               x.Required == y.Required &&
               x.Disabled == y.Disabled &&
               x.Omitted == y.Omitted &&
               x.Localized == y.Localized &&
               AreValidationsEqual(x.Validations, y.Validations);
    }

    public int GetHashCode([DisallowNull] Field obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (obj.Name?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.Type?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.LinkType?.GetHashCode() ?? 0);
            hash = hash * 23 + obj.Required.GetHashCode();
            hash = hash * 23 + obj.Disabled.GetHashCode();
            hash = hash * 23 + obj.Omitted.GetHashCode();
            hash = hash * 23 + obj.Localized.GetHashCode();
            return hash;
        }
    }

    private static bool AreValidationsEqual(List<IFieldValidator>? x, List<IFieldValidator>? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null || x.Count != y.Count)
            return false;

        return x.SequenceEqual(y, ValidatorComparer);
    }
}