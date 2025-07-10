namespace Contool.Console.Infrastructure.Utils.Extensions;

public static class StringExtensions
{
    public static string Snip(this string text, int snipTo)
    {
        if (text.Length <= snipTo) return text;

        return text[..(snipTo - 1)] + "..";
    }
}
