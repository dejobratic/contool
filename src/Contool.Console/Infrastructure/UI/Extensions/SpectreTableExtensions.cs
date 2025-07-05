using Spectre.Console;

namespace Contool.Console.Infrastructure.UI.Extensions;

public static class SpectreTableExtensions
{
    public static Table AddEmptyColumn(this Table table)
        => table.AddColumn(string.Empty);
}