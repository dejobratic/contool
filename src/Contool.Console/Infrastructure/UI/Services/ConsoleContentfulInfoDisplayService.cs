using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Spectre.Console;
using Environment = Contentful.Core.Models.Management.ContentfulEnvironment;
using Table = Spectre.Console.Table;
using Text = Spectre.Console.Text;

namespace Contool.Console.Infrastructure.UI.Services;

public class ConsoleContentfulInfoDisplayService : IContentfulInfoDisplayService
{
    public async Task DisplayInfoAsync(IContentfulLoginService contentfulService)
    {
        var (space, environment, user, locales, contentTypes) = await LoadInfoAsync(contentfulService);

        var spaceInfoTable = BuildSpaceInfoTable(space, environment, user);
        AnsiConsole.Write(spaceInfoTable);
        AnsiConsole.WriteLine();

        var contentTypesInfoTable = BuildContentTypesInfoTable(contentTypes.ToList());
        AnsiConsole.Write(contentTypesInfoTable);
        AnsiConsole.WriteLine();

        var localesInfoTable = BuildLocalesInfoTable(locales.ToList());
        AnsiConsole.Write(localesInfoTable);
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
                new TableColumn(new Text("Contentful", Styles.Heading)),
                new TableColumn(new Text(string.Empty)),
                new TableColumn(new Text(string.Empty)))
            .AddRow(
                new Text("  Space", Styles.Soft),
                new Text(" : ", Styles.Dim),
                new Markup($"{space.GetId()} [{Styles.Dim.ToMarkup()}]({space.Name})[/]", Styles.Alert))
            .AddRow(
                new Text("  Environment", Styles.Soft),
                new Text(" : ", Styles.Dim),
                new Markup($"{environment.GetId()}", Styles.Alert))
            .AddRow(
                new Text("  User", Styles.Soft),
                new Text(" : ", Styles.Dim),
                new Markup($"{user.Email} [{Styles.Dim.ToMarkup()}]({user.GetId()})[/]", Styles.Normal));
    }

    private static Table BuildContentTypesInfoTable(List<ContentTypeExtended> contentTypes)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup($"Content Types [{Styles.Dim.ToMarkup()}]({contentTypes.Count})[/]", Styles.Heading)))
            .AddColumn(string.Empty)
            .AddColumn(string.Empty);

        foreach (var type in contentTypes)
        {
            table.AddRow(
                new Markup($"  {type.Name.Trim()}", Styles.Soft),
                new Markup(" : ", Styles.Dim),
                new Markup($"{type.GetId()} [{Styles.Dim.ToMarkup()}]({type.TotalEntries})[/]", Styles.Alert));
        }

        return table;
    }

    private static Table BuildLocalesInfoTable(List<Locale> locales)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Markup($"Locales [{Styles.Dim.ToMarkup()}]({locales.Count})[/]", Styles.Heading)))
            .AddColumn(string.Empty)
            .AddColumn(string.Empty);

        var defaultLocale = locales.FirstOrDefault(l => l.Default);

        if (defaultLocale is not null)
        {
            table.AddRow(
                new Markup($"  {defaultLocale.Name}", Styles.Normal),
                new Markup(" : ", Styles.Dim),
                new Markup($"{defaultLocale.Code} [{Styles.Dim.ToMarkup()}](default)[/]", Styles.Alert));
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