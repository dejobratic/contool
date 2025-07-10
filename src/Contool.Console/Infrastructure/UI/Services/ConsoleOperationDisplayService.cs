using Contool.Console.Infrastructure.UI.Extensions;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI.Services;

public class ConsoleOperationDisplayService : IOperationsDisplayService
{
    public void DisplayOperations(OperationTrackResult result)
    {
        var summary = CreateOperationSummaryTable(result);
        
        AnsiConsole.Write(summary);
        AnsiConsole.WriteLine();

        if (result.Operations.Count == 0)
        {
            AnsiConsole.Write(new Text("  No operations performed.", Styles.Alert));
            AnsiConsole.WriteLine();
            return;
        }

        var details = CreateOperationDetailsTable(result);
        
        AnsiConsole.Write(details);
    }

    private static Table CreateOperationSummaryTable(OperationTrackResult result)
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

    private static Table CreateOperationDetailsTable(OperationTrackResult result)
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