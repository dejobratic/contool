using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.UI.Extensions;
using Contool.Console.Infrastructure.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands;

public abstract class CommandBase<TSettings> : AsyncCommand<TSettings>
    where TSettings : CommandSettings
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        DisplayCommandDetails(context, settings);

        var profiledResult = await ExecutionProfiler.ProfileAsync(
            () => ExecuteCommandAsync(context, settings));

        DisplayCommandExecutionMetrics(profiledResult);

        return profiledResult.Result;
    }

    private static void DisplayCommandDetails(CommandContext context, TSettings settings)
    {
        var commandDetails = BuildCommandDetailsTable(context, settings);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(commandDetails);
        AnsiConsole.WriteLine();
    }

    private static Table BuildCommandDetailsTable(CommandContext context, TSettings settings)
    {
        var command = GetCommand(context);
        var options = GetCommandOptions(settings);

        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Text("Command", Styles.Normal)))
            .AddColumn(new TableColumn(new Text(" : ", Styles.Dim)))
            .AddColumn(new TableColumn(new Markup(command, Styles.Alert)));

        if (options.Count == 0)
        {
            table.AddRow(
                new Text("Command does not have any options.", Styles.Alert),
                new Text(string.Empty),
                new Text(string.Empty));
        }
        else
        {
            table.AddRow(
                new Text("Options", Styles.Normal),
                new Text(string.Empty),
                new Text(string.Empty));

            foreach (var (option, value) in options)
            {
                var displayValue = value;

                if (value is string[] stringArray)
                {
                    displayValue = string.Join(',', stringArray.Select(e => $"'{e}'"));
                }

                var valueMarkup = displayValue?.ToString().EscapeMarkup() is null
                    ? new Text("null", Styles.Dim)
                    : new Text(displayValue.ToString().EscapeMarkup(), Styles.Alert);

                table.AddRow(
                    new Markup($"--{option}", Styles.Dim).RightJustified(),
                    new Markup(" : ", Styles.Dim),
                    valueMarkup);
            }
        }

        return table;
    }

    private static string GetCommand(CommandContext context)
    {
        var commandParts = context.Arguments
            .TakeWhile(arg => !arg.StartsWith('-'))
            .ToArray();

        return string.Join(' ', commandParts).EscapeMarkup();
    }

    private static Dictionary<string, object?> GetCommandOptions(TSettings settings)
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
                .LongNames
                .ToArray();

            if (attr is null || attr.Length == 0)
                continue;

            var option = attr[0];
            var value = prop.GetValue(settings);

            result.Add(option, value);
        }

        return result;
    }

    private static void DisplayCommandExecutionMetrics(MeasuredResult<int> profiledResult)
    {
        var table = new Table()
            .NoBorder()
            .AddColumn(new TableColumn(new Text("Profiling", Styles.Normal)))
            .AddEmptyColumn()
            .AddEmptyColumn()
            .AddRow(
                new Text("  Execution Time", Styles.Normal),
                new Text(" : ", Styles.Dim),
                new Text($"{profiledResult.FormattedElapsedTime}", Styles.Normal))
            .AddRow(
                new Text("  Peak Memory Usage", Styles.Normal),
                new Text(" : ", Styles.Dim),
                new Text($"{profiledResult.FormatedMemoryUsage}", Styles.Normal));
        
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    protected abstract Task<int> ExecuteCommandAsync(CommandContext context, TSettings settings);
}
