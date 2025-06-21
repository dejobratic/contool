using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands;

public abstract class CommandBase<TSettings>(
    IRuntimeContext runtimeContext) : AsyncCommand<TSettings>
    where TSettings : SettingsBase
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        // TODO: refactor
        var isDryRun = settings is WriteSettingsBase writeSettins && writeSettins.Apply is false;
        runtimeContext.SetDryRun(isDryRun);

        DisplaySettings(context, settings);

        var profiledResult = await ExecutionProfiler.ProfileAsync(
            async () => await ExecuteInternalAsync(context, settings));

        DisplayMetrics(profiledResult);

        return profiledResult.Result;
    }

    private static void DisplaySettings(CommandContext context, TSettings settings)
    {
        var command = BuildCommandHeader(context);
        var optionTable = BuildOptionTable(settings);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(command);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
        AnsiConsole.Write(optionTable);
        AnsiConsole.WriteLine();
    }

    private static Table BuildOptionTable(TSettings settings)
    {
        var optionTable = new Table().NoBorder();

        optionTable.AddColumn(new TableColumn(new Text("Option", Styles.AlertAccent)).Alignment(Justify.Right));
        optionTable.AddColumn(new TableColumn(new Text("", Styles.AlertAccent)));
        optionTable.AddColumn(new TableColumn(new Text("Value", Styles.AlertAccent)));

        var options = GetOptions(settings);

        foreach (var (option, value) in options)
        {
            var displayValue = value;

            if (value is string[] stringArray)
            {
                displayValue = string.Join(',', stringArray.Select(e => $"'{e}'"));
            }

            optionTable.AddRow(
                new Markup($"--{option}", Styles.Dim),
                new Markup($"=", Styles.Dim),
                new Markup($"{displayValue?.ToString().EscapeMarkup()}", Styles.Normal));
        }

        return optionTable;
    }

    private static Dictionary<string, object?> GetOptions(TSettings settings)
    {
        var result = new Dictionary<string, object?>();

        foreach (var prop in settings.GetType().GetProperties())
        {
            // Skip properties marked with [Secret]
            if (prop.IsDefined(typeof(SecretAttribute), inherit: true))
                continue;

            var attr = prop
                .GetCustomAttributes(typeof(CommandOptionAttribute), inherit: true)
                .Cast<CommandOptionAttribute>()
                .FirstOrDefault()?
                .LongNames.ToArray();

            if (attr is not null && attr.Length > 0)
            {
                var option = attr[0];
                if (option is not null)
                {
                    var value = prop.GetValue(settings);
                    result.Add(option, value);
                }
            }
        }

        return result;
    }

    private static Markup BuildCommandHeader(CommandContext context)
    {
        var command = GetCommand(context);
        return new Markup(command, Styles.Alert);
    }

    private static string GetCommand(CommandContext context)
    {
        var commandParts = context.Arguments
            .TakeWhile(arg => !arg.StartsWith('-'))
            .ToArray();

        return string.Join(' ', commandParts).EscapeMarkup();
    }

    private static void DisplayMetrics(MeasuredResult<int> profiledResult)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold {Styles.Dim.Foreground}]Execution Time:[/] {profiledResult.FormattedElapsedTime}");
        AnsiConsole.MarkupLine($"[bold {Styles.Dim.Foreground}]Peak Memory Usage:[/] {profiledResult.FormatedMemoryUsage}");
        AnsiConsole.WriteLine();
    }

    protected abstract Task<int> ExecuteInternalAsync(CommandContext context, TSettings settings);
}
