using Contool.Console.Infrastructure.UI.Extensions;
using Contool.Console.Infrastructure.Utils;
using Contool.Console.Infrastructure.Utils.Models;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI.Services;

public class ConsoleCommandInfoDisplayService : ICommandInfoDisplayService
{
    public void DisplayCommand(string command, Dictionary<string, object?> options)
    {
        var info = CreateCommandInfoTable(command, options);
        
        AnsiConsole.WriteLine();
        AnsiConsole.Write(info);
        AnsiConsole.WriteLine();
    }

    public void DisplayExecutionMetrics(MeasuredResult<int> result)
    {
        var info = CreateExecutionMetricsInfoTable(result);
        
        AnsiConsole.WriteLine();
        AnsiConsole.Write(info);
        AnsiConsole.WriteLine();
    }

    private static Table CreateCommandInfoTable(string command, Dictionary<string, object?> options)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Text("Command", Styles.Normal)))
            .AddColumn(new TableColumn(new Text(" : ", Styles.Dim)))
            .AddColumn(new TableColumn(new Markup(command, Styles.Alert)));

        if (options.Count == 0)
        {
            table.AddRow(
                new Text("Command does not have any options.", Styles.Alert),
                new Text(string.Empty),
                new Text(string.Empty));
        }
        else
        {
            table.AddRow(
                new Text("Options", Styles.Normal),
                new Text(string.Empty),
                new Text(string.Empty));

            foreach (var (option, value) in options)
            {
                var displayValue = value;

                if (value is string[] stringArray)
                {
                    displayValue = string.Join(',', stringArray.Select(e => $"'{e}'"));
                }

                var valueMarkup = displayValue?.ToString().EscapeMarkup() is null
                    ? new Text("null", Styles.Dim)
                    : new Text(displayValue.ToString().EscapeMarkup(), Styles.Alert);

                table.AddRow(
                    new Markup($"--{option}", Styles.Dim).RightJustified(),
                    new Markup(" : ", Styles.Dim),
                    valueMarkup);
            }
        }

        return table;
    }
    
    private static Table CreateExecutionMetricsInfoTable(MeasuredResult<int> result)
    {
        return new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Text("Profiling", Styles.Normal)))
            .AddEmptyColumn()
            .AddEmptyColumn()
            .AddRow(
                new Text("  Execution Time", Styles.Normal),
                new Text(" : ", Styles.Dim),
                new Text($"{result.FormattedElapsedTime}", Styles.Normal))
            .AddRow(
                new Text("  Peak Memory Usage", Styles.Normal),
                new Text(" : ", Styles.Dim),
                new Text($"{result.FormatedMemoryUsage}", Styles.Normal));
    }
}