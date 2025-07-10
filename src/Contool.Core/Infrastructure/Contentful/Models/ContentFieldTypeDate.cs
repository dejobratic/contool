using Contentful.Core.Models;
using System.Globalization;

namespace Contool.Core.Infrastructure.Contentful.Models;

internal class ContentFieldTypeDate() : ContentFieldType("Date")
{
    public override Type DotnetType => typeof(DateTime);

    public override object GetValue(object prop, Schema? _)
        => DateTime.Parse(prop.ToString()!);

    public override object? Deserialize(object? raw, Schema? _)
        => ToDateTime(raw);

    public override bool IsValidRawValue(object? value)
        => DateTime.TryParse(value?.ToString(), out _);

    private static DateTime? ToDateTime(object? value)
    {
        if (value is null) return null;

        var date = FromInvariantDateTime(value);

        if (date == DateTime.MinValue) return null;

        // Contentful doesn't support milliseconds, although it states it is ISO 8601 compliant :(
        return StripMilliseconds(date);
    }

    private static DateTime FromInvariantDateTime(object value)
    {
        return value switch
        {
            null => default,
            DateTime dateTime => dateTime,
            string dateTimeString => Convert.ToDateTime(dateTimeString, CultureInfo.InvariantCulture),
            _ => Convert.ToDateTime(value)
        };
    }

    private static DateTime StripMilliseconds(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
    }
}
