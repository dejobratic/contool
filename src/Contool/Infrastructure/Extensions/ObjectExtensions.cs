using System.Globalization;

namespace Contool.Infrastructure.Extensions;

public static class ObjectExtensions
{
    public static DateTime FromInvariantDateTime(this object value)
    {
        return value switch
        {
            null => default,
            DateTime dateTime => dateTime,
            string dateTimeString => Convert.ToDateTime(dateTimeString, CultureInfo.InvariantCulture),
            _ => Convert.ToDateTime(value)
        };
    }
}