using Contentful.Core.Models.Management;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Contool.Core.Infrastructure.Contentful.Utils;

public class ContentTypeFieldValidatorEqualityComparer : IEqualityComparer<IFieldValidator>
{
    public bool Equals(IFieldValidator? x, IFieldValidator? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        // Compare types
        if (x.GetType() != y.GetType())
            return false;

        // Fallback to reflection-based comparison
        return ArePropertiesEqual(x, y);
    }

    public int GetHashCode([DisallowNull] IFieldValidator obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + obj.GetType().GetHashCode();

            // Include all properties in hash calculation
            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(obj);
                hash = hash * 23 + (value?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

    private static bool ArePropertiesEqual(IFieldValidator x, IFieldValidator y)
    {
        var type = x.GetType();

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var xValue = property.GetValue(x);
            var yValue = property.GetValue(y);

            if (!Equals(xValue, yValue))
                return false;
        }

        return true;
    }
}
