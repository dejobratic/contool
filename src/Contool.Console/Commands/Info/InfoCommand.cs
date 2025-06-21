using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Console.Infrastructure.Extensions;
using Contool.Console.Infrastructure.UI;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;

using Environment = Contentful.Core.Models.Management.ContentfulEnvironment;
using Table = Spectre.Console.Table;
using Text = Spectre.Console.Text;

namespace Contool.Console.Commands.Info;

public sealed class InfoCommand(
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder)
    : CommandBase<InfoCommand.Settings>(runtimeContext)
{
    public class Settings : SettingsBase { }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        var contentfulService = contentfulServiceBuilder
            .WithSpaceId(settings.SpaceId)
            .WithEnvironmentId(settings.EnvironmentId)
            .Build();

        var (space, environment, user, locales, contentTypes) = await LoadInfoAsync(contentfulService);

        var spaceInfoTable = BuildSpaceInfoTable(space, environment, user);
        AnsiConsole.Write(spaceInfoTable);

        var mainTable = BuildMainTable(locales, contentTypes);
        AnsiConsole.Write(mainTable);

        return 0;
    }

    private static async Task<(Space space, Environment environment, User user, IEnumerable<Locale> locales, IEnumerable<ContentTypeExtended> contentTypes)> LoadInfoAsync(
        IContentfulLoginService contentfulService)
    {
        var spaceTask = contentfulService.GetDefaultSpaceAsync();
        var environmentTask = contentfulService.GetDefaultEnvironmentAsync();
        var userTask = contentfulService.GetCurrentUserAsync();
        var localesTask = contentfulService.GetLocalesAsync();
        var contentTypesTask = contentfulService.GetContentTypeExtendedAsync();

        await Task.WhenAll(spaceTask, environmentTask, userTask, localesTask, contentTypesTask);

        return
            (await spaceTask,
            await environmentTask,
            await userTask,
            await localesTask,
            (await contentTypesTask).OrderBy(t => t.Name));
    }

    private static Table BuildSpaceInfoTable(Space space, Environment environment, User user)
    {
        return new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn(new TableColumn(new Text("Space Id", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("Space Name", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("Environment", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("User Id", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("User Name", Styles.AlertAccent)))
            .AddRow(
                new Markup(space.GetId(), Styles.Alert),
                new Markup(space.Name, Styles.Normal),
                new Markup(environment.SystemProperties.Id, Styles.Alert),
                new Markup(user.GetId(), Styles.Normal),
                new Markup(user.Email, Styles.Normal)
            );
    }

    private static Table BuildMainTable(IEnumerable<Locale> locales, IEnumerable<ContentTypeExtended> contentTypes)
    {
        return new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn(new TableColumn(new Text("Content Types", Styles.AlertAccent)))
            .AddColumn(new TableColumn(new Text("Locales", Styles.AlertAccent)))
            .AddRow(BuildContentTypesInfoTable(contentTypes), BuildLocalesInfoTable(locales));
    }

    private static Table BuildContentTypesInfoTable(IEnumerable<ContentTypeExtended> contentTypes)
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn("Type Name")
            .AddColumn("Type Id")
            //.AddColumn("Field #", column => column.RightAligned())
            .AddColumn("Record #", column => column.RightAligned());

        foreach (var type in contentTypes)
        {
            table.AddRow(
                new Markup(type.Name.Trim().Snip(28), Styles.Normal),
                new Markup(type.GetId(), Styles.Alert),
                //new Markup(ct.Fields.Count.ToString(), Styles.Normal),
                new Markup(type.TotalEntries.ToString(), Styles.Normal));
        }

        return table;
    }

    private static Table BuildLocalesInfoTable(IEnumerable<Locale> locales)
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Styles.Dim.Foreground)
            .AddColumn("Name")
            .AddColumn("Code");

        var defaultLocale = locales.FirstOrDefault(l => l.Default);

        if (defaultLocale is not null)
        {
            table.AddRow(
                new Markup(defaultLocale.Name, Styles.Normal),
                new Markup(defaultLocale.Code, Styles.Alert));
        }

        foreach (var locale in locales.Where(l => !l.Default).OrderBy(l => l.Name))
        {
            table.AddRow(
                new Markup(locale.Name, Styles.Normal),
                new Markup(locale.Code, locale.Default ? Styles.Alert : Styles.Normal));
        }

        return table;
    }
}