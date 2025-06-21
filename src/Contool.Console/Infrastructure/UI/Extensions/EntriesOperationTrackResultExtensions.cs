using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;

namespace Contool.Console.Infrastructure.UI.Extensions;

public static class EntriesOperationTrackResultExtensions
{
    public static void DrawTable(this EntriesOperationTrackResults? result)
    {
        if (result is null)
            return;

        var table = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn(new TableColumn(new Text("Operation", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("Record #", Styles.AlertAccent)))
            .AddRow(
                new Markup("Created or updated", Styles.Normal),
                new Markup(result.CreatedOrUpdatedCount.ToString(), Styles.Alert))
            .AddRow(
                new Markup("Published", Styles.Normal),
                new Markup(result.PublishedCount.ToString(), Styles.Alert))
            .AddRow(
                new Markup("Unpublished", Styles.Normal),
                new Markup(result.UnpublishedCount.ToString(), Styles.Alert))
            .AddRow(
                new Markup("Archived", Styles.Normal),
                new Markup(result.ArchivedCount.ToString(), Styles.Alert))
            .AddRow(
                new Markup("Unarchived", Styles.Normal),
                new Markup(result.UnarchivedCount.ToString(), Styles.Alert))
            .AddRow(
                new Markup("Deleted", Styles.Normal),
                new Markup(result.DeletedCount.ToString(), Styles.Alert));

        var mainTable = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn(new TableColumn(new Text("Entry operations", Styles.AlertAccent)))
            .AddRow(table);



        AnsiConsole.WriteLine();
        AnsiConsole.Write(mainTable);
        AnsiConsole.WriteLine();
    }
}
