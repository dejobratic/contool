using Contool.Console.Infrastructure.UI.Extensions;
using Contool.Console.Infrastructure.Utils.Extensions;
using Contool.Core.Infrastructure.Validation;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI.Services;

public class ConsoleValidationSummaryDisplayService : IValidationSummaryDisplayService
{
    public void DisplayValidationSummary(EntryValidationSummary summary, int totalEntries)
    {
        var summaryTable = CreateSummaryTable(summary, totalEntries);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(summaryTable);
        

        if (summary.Errors.Count > 0)
        {
            var errorsTable = CreateErrorsTable(summary.Errors);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(errorsTable);
        }

        if (summary.Warnings.Count > 0)
        {
            var warningsTable = CreateWarningsTable(summary.Warnings);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(warningsTable);
        }
    }

    private static Table CreateSummaryTable(EntryValidationSummary summary, int totalEntries)
    {
        var successRate = totalEntries > 0 ? (double)summary.ValidEntries.Count / totalEntries * 100 : 0;

        var table = new Table()
            .NoBorder()
            .AddColumns(new TableColumn(new Text("Validation Summary", Styles.Heading)))
            .AddEmptyColumn()
            .AddEmptyColumn()
            .AddRow(
                new Text("  Total Processed", Styles.Soft),
                new Text(" : ", Styles.Dim),
                new Markup($"{totalEntries}", Styles.Alert))
            .AddRow(
                new Text("  Total Valid", Styles.Soft),
                new Text(" : ", Styles.Dim),
                new Markup($"{summary.ValidEntries.Count}", Styles.Alert))
            .AddRow(
                new Text("  Success Rate", Styles.Soft),
                new Text(" : ", Styles.Dim),
                new Markup($"{successRate:F1}%", Styles.Alert));

        return table;
    }

    private static Table CreateErrorsTable(List<ValidationError> errors)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup($"  Errors [{Styles.Dim.Foreground}]({errors.Count})[/]", Styles.Heading)))
            .AddEmptyColumn()
            .AddEmptyColumn();

        foreach (var errorGroup in errors.GroupBy(e => e.Type))
        {
            table.AddRow(
                new Markup($"    {errorGroup.Key.ToScreamingSnakeCase()}", Styles.Soft),
                new Markup(" : ", Styles.Dim),
                new Markup($"[{Styles.Alert.ToMarkup()}]{errorGroup.Count()}[/] errors", Styles.Dim));
        }

        return table;
    }

    private static Table CreateWarningsTable(List<ValidationWarning> warnings)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup($"  Warnings [{Styles.Dim.Foreground}]({warnings.Count})[/]", Styles.Heading)))
            .AddEmptyColumn()
            .AddEmptyColumn();

        foreach (var warningGroup in warnings.GroupBy(w => w.Type))
        {
            table.AddRow(
                new Markup($"    {warningGroup.Key.ToScreamingSnakeCase()}", Styles.Soft),
                new Markup(" : ", Styles.Dim),
                new Markup($"[{Styles.Alert.ToMarkup()}]{warningGroup.Count()}[/] warnings", Styles.Dim));
        }

        return table;
    }
}