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

        if (result.Operations.Count == 0)
        {
            AnsiConsole.Write(new Text("  No operations performed.", Styles.Alert));
            AnsiConsole.WriteLine();
            return;
        }
        
        AnsiConsole.Write(result.ToOperationDetailsTable());
    }

    private static Table ToOperationSummaryTable(this OperationTrackResult result)
    {
        var totalProcessed = result.TotalEntries;
        var totalSucceeded = result.SuccessfulEntries;
        var successRate = totalProcessed > 0 ? (double)totalSucceeded / totalProcessed * 100 : 0;

        var table = new Table()
            .NoBorder()
            .AddColumns(new TableColumn(new Text("Summary", Styles.Normal)))
            .AddEmptyColumn()
            .AddEmptyColumn()
            .AddRow(
                new Text("  Total Processed", Styles.Normal),
                new Text(" : ", Styles.Dim),
                new Markup($"{totalProcessed}", Styles.Alert))
            .AddRow(
                new Text("  Success Rate", Styles.Normal),
                new Text(" : ", Styles.Dim),
                new Markup($"{successRate:F1}%", Styles.Alert));

        return table;
    }

    private static Table ToOperationDetailsTable(this OperationTrackResult result)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup("Operations", Styles.Normal)))
            .AddEmptyColumn()
            .AddEmptyColumn()
            .AddEmptyColumn();

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