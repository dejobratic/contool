using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI.Extensions;

public static class EntriesOperationTrackResultExtensions
{
    public static Table DrawTable(this EntriesOperationTrackResults result)
    {
        return new Table()
             .RoundedBorder()
             .BorderColor(Styles.Dim.Foreground)
             .AddColumn(new TableColumn(new Text("Entry operations", Styles.AlertAccent)))
             .AddRow(result.ToOperationsTable());
    }

    private static Table ToOperationsTable(this EntriesOperationTrackResults result)
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn(new TableColumn(new Text("Operation", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("Succeded #", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("Failed #", Styles.AlertAccent)));

        foreach (var operation in result.Operations)
            table.AddRow(
                new Markup(operation.Key.Name, Styles.Normal),
                new Markup(operation.Value.SuccessCount.ToString(), Styles.Alert),
                new Markup(operation.Value.ErrorCount.ToString(), Styles.Alert));

        return table;
    }
}
