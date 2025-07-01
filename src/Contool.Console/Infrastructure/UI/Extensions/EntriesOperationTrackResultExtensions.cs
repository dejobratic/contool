using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Rendering;
using Table = Spectre.Console.Table;
using Text = Spectre.Console.Text;

namespace Contool.Console.Infrastructure.UI.Extensions;

public static class EntriesOperationTrackResultExtensions
{
    public static void DrawTable(this OperationTrackResult result)
    {
        AnsiConsole.Write(result.ToOperationSummaryTable());
        AnsiConsole.WriteLine();
        AnsiConsole.Write(result.ToOperationDetailsTable());
    }

    private static Table ToOperationSummaryTable(this OperationTrackResult result)
    {
        var totalProcessed = result.TotalEntries;
        var totalSucceeded = result.SuccessfulEntries;
        var successRate = totalProcessed > 0 ? (double)totalSucceeded / totalProcessed * 100 : 0;

        var table = new Table()
            .NoBorder()
            .AddColumns(
                new TableColumn(new Text("Summary", Styles.Normal)),
                new TableColumn(new Text(string.Empty)),
                new TableColumn(new Text(string.Empty)))
            .AddRow(
                new Text("  Total Processed", Styles.Normal),
                new Text(" : ", Styles.Dim),
                new Markup($"{totalProcessed}", Styles.Alert))
            .AddRow(
                new Text("  Success Rate"),
                new Text(" : ", Styles.Dim),
                new Markup($"{successRate:F1}%", Styles.Alert));

        return table;
    }

    private static Renderable ToOperationDetailsTable(this OperationTrackResult result)
    {
        return result.Operations.Count == 0
            ? new Markup("  No operations performed.", Styles.Alert)
            : result.ToOperationsTable();
    }

    private static Table ToOperationsTable(this OperationTrackResult result)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup("Operations", Styles.Normal)))
            .AddColumn(new TableColumn(new Markup(string.Empty)))
            .AddColumn(new TableColumn(new Markup(string.Empty)))
            .AddColumn(new TableColumn(new Markup(string.Empty)));

        foreach (var operation in result.Operations)
        {
            table.AddRow(
                new Markup($"  {operation.Key.Name}", Styles.Normal),
                new Markup(" : ", Styles.Dim),
                new Markup($"[{Styles.Alert.ToMarkup()}]{operation.Value.SuccessCount}[/] succeeded", Styles.Dim),
                new Markup($"[{Styles.Alert.ToMarkup()}]{operation.Value.ErrorCount}[/] failed", Styles.Dim));
        }

        return table;
    }
}