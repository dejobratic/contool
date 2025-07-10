using System.Text.RegularExpressions;

namespace Contool.Console.Infrastructure.Utils.Extensions;

public static partial class StringExtensions
{
    public static string Snip(this string text, int snipTo)
    {
        if (text.Length <= snipTo) return text;

        return text[..(snipTo - 1)] + "..";
    }
    
    public static string ToScreamingSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return StringRegex().Replace(input, "_")
            .Replace(' ', '_')
            .Replace('-', '_')
            .ToUpperInvariant();
    }

    [GeneratedRegex(@"(?<!^)(?=[A-Z])")]
    private static partial Regex StringRegex();
}
