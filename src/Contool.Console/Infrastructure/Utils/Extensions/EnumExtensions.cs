namespace Contool.Console.Infrastructure.Utils.Extensions;

public static class EnumExtensions
{
    public static string ToScreamingSnakeCase(this Enum enumValue)
    {
        return enumValue.ToString().ToScreamingSnakeCase();
    }
}