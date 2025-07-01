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
    : LoggedInCommandBase<InfoCommand.Settings>(runtimeContext, contentfulServiceBuilder)
{
    private readonly IContentfulLoginServiceBuilder _contentfulServiceBuilder = contentfulServiceBuilder;

    public class Settings : SettingsBase { }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var contentfulService = _contentfulServiceBuilder
            .WithSpaceId(settings.SpaceId)
            .WithEnvironmentId(settings.EnvironmentId)
            .Build();

        var (space, environment, user, locales, contentTypes) = await LoadInfoAsync(contentfulService);

        var spaceInfoTable = BuildSpaceInfoTable(space, environment, user);
        AnsiConsole.Write(spaceInfoTable);
        AnsiConsole.WriteLine();

        var contentTypesInfoTable = BuildContentTypesInfoTable(contentTypes.ToList());
        AnsiConsole.Write(contentTypesInfoTable);
        AnsiConsole.WriteLine();

        var localesInfoTable = BuildLocalesInfoTable(locales.ToList());
        AnsiConsole.Write(localesInfoTable);

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
            .NoBorder()
            .AddColumns(
                new TableColumn(new Text("Contentful", Styles.Normal)),
                new TableColumn(new Text(string.Empty)),
                new TableColumn(new Text(string.Empty)))
            .AddRow(
                new Text("  Space"),
                new Text(" : ", Styles.Dim),
                new Markup($"{space.GetId()} [{Styles.Dim.ToMarkup()}]({space.GetId()})[/]", Styles.Alert))
            .AddRow(
                new Text("  Env"),
                new Text(" : ", Styles.Dim),
                new Markup($"{environment.GetId()}", Styles.Alert))
            .AddRow(
                new Text("  User"),
                new Text(" : ", Styles.Dim),
                new Markup($"{user.Email} [{Styles.Dim.ToMarkup()}]({user.GetId()})[/]", Styles.Normal));
    }

    private static Table BuildContentTypesInfoTable(List<ContentTypeExtended> contentTypes)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup($"Content Types [{Styles.Dim.ToMarkup()}]({contentTypes.Count})[/]", Styles.Normal)))
            .AddColumn(string.Empty)
            .AddColumn(string.Empty);

        foreach (var type in contentTypes)
        {
            table.AddRow(
                new Markup($"  {type.Name.Trim()}", Styles.Normal),
                new Markup(" : ", Styles.Dim),
                new Markup($"{type.GetId()} [{Styles.Dim.ToMarkup()}]({type.TotalEntries})[/]", Styles.Alert));
        }

        return table;
    }

    private static Table BuildLocalesInfoTable(List<Locale> locales)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup($"Locales [{Styles.Dim.ToMarkup()}]({locales.Count})[/]", Styles.Normal)))
            .AddColumn(string.Empty)
            .AddColumn(string.Empty);

        var defaultLocale = locales.FirstOrDefault(l => l.Default);

        if (defaultLocale is not null)
        {
            table.AddRow(
                new Markup($"  {defaultLocale.Name}", Styles.Normal),
                new Markup(" : ", Styles.Dim),
                new Markup($"{defaultLocale.Code}", Styles.Alert));
        }

        foreach (var locale in locales.Where(l => !l.Default).OrderBy(l => l.Name))
        {
            table.AddRow(
                new Markup($"  {locale.Name}", Styles.Normal),
                new Markup(" : ", Styles.Dim),
                new Markup(locale.Code, Styles.Normal));
        }

        return table;
    }
}