using System.Globalization;

namespace Contool.Extensions;

public static class ObjectExtensions
{
    public static DateTime FromInvariantDateTime(this object value)
    {
        if(value is null)
        {
            return default;
        }

        else if (value is DateTime dateTime)
        {
            return dateTime;
        }

        else if (value is string dateTimeString)
        {
            return Convert.ToDateTime(dateTimeString, CultureInfo.InvariantCulture);
        }

        return Convert.ToDateTime(value);
    }
}