using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Cli.Infrastructure.Extensions;
using Contool.Cli.Infrastructure.UI;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Spectre.Console;
using Spectre.Console.Cli;

using Environment = Contentful.Core.Models.Management.ContentfulEnvironment;
using Table = Spectre.Console.Table;
using Text = Spectre.Console.Text;

namespace Contool.Cli.Commands.Info;

public sealed class InfoCommand(
    IContentfulLoginService contentfulService) : AsyncCommand<InfoCommand.Settings>
{
    public class Settings : SettingsBase { }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var (space, environment, user, locales, contentTypes) = await LoadInfoAsync();

        AnsiConsole.Write(BuildTopTable(space, environment, user));

        var contentTypesTable = BuildContentTypesTable(contentTypes);
        var localesTable = BuildLocalesTable(locales);

        var mainTable = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn(new TableColumn(new Text("Content Types", Styles.SubHeading)))
            .AddColumn(new TableColumn(new Text("Locales", Styles.SubHeading)))
            .AddRow(contentTypesTable, localesTable);

        AnsiConsole.Write(mainTable);

        return 0;
    }

    private async Task<(Space space, Environment environment, User user, IEnumerable<Locale> locales, IEnumerable<ContentTypeExtended> contentTypes)> LoadInfoAsync()
    {
        var spaceTask = contentfulService.GetDefaultSpaceAsync();
        var environmentTask = contentfulService.GetDefaultEnvironmentAsync();
        var userTask = contentfulService.GetCurrentUserAsync();
        var localesTask = contentfulService.GetLocalesAsync();
        var contentTypesTask = contentfulService.GetContentTypeExtendedAsync();

        await Task.WhenAll(spaceTask, environmentTask, userTask, localesTask, contentTypesTask);

        return (
            await spaceTask,
            await environmentTask,
            await userTask,
            await localesTask,
            (await contentTypesTask).OrderBy(t => t.Name)
        );
    }

    private static Table BuildTopTable(Space space, Environment environment, User user)
    {
        return new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumns("Space", "Id", "Environment", "User Id", "User Name")
            .AddRow(
                new Markup(space.Name, Styles.Alert),
                new Markup(space.GetId(), Styles.Normal),
                new Markup(environment.SystemProperties.Id, Styles.Normal),
                new Markup(user.GetId(), Styles.Normal),
                new Markup(user.Email, Styles.Normal)
            );
    }

    private static Table BuildContentTypesTable(IEnumerable<ContentTypeExtended> contentTypes)
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn("Type Name")
            .AddColumn("Content Id")
            .AddColumn("Field #", column => column.RightAligned())
            .AddColumn("Record #", column => column.RightAligned());

        foreach (var ct in contentTypes)
        {
            table.AddRow(
                new Markup(ct.Name.Trim().Snip(27), Styles.Normal),
                new Markup(ct.GetId(), Styles.AlertAccent),
                new Markup(ct.Fields.Count.ToString(), Styles.Normal),
                new Markup(ct.TotalEntries.ToString(), Styles.Normal));
        }

        return table;
    }

    private static Table BuildLocalesTable(IEnumerable<Locale> locales)
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn("Name")
            .AddColumn("Code");

        foreach (var locale in locales.OrderBy(l => l.Name))
        {
            table.AddRow(
                new Markup(locale.Name, Styles.Normal),
                new Markup(locale.Code, Styles.AlertAccent));
        }

        return table;
    }
}