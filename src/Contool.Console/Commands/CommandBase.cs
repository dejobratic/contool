using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.UI.Extensions;
using Contool.Console.Infrastructure.UI.Services;
using Contool.Console.Infrastructure.Utils;
using Contool.Console.Infrastructure.Utils.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Contool.Console.Commands;

public abstract class CommandBase<TSettings>(
    ICommandInfoDisplayService commandInfoDisplayService,
    IErrorDisplayService errorDisplayService) : AsyncCommand<TSettings>
    where TSettings : CommandSettings
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        DisplayCommandDetails(context, settings);

        var profiledResult = await ExecutionProfiler.ProfileAsync(
            () => ExecuteCommandWithErrorHandlingAsync(context, settings));

        DisplayCommandExecutionMetrics(profiledResult);

        return profiledResult.Result;
    }

    private async Task<int> ExecuteCommandWithErrorHandlingAsync(CommandContext context, TSettings settings)
    {
        try
        {
            return await ExecuteCommandAsync(context, settings);
        }
        catch (Exception ex)
        {
            errorDisplayService.DisplayError(ex);
            return CommandResult.Error;
        }
    }
    
    private void DisplayCommandDetails(CommandContext context, TSettings settings)
    {
        var command = GetCommand(context);
        var options = GetCommandOptions(settings);
        
        commandInfoDisplayService.DisplayCommand(command, options);
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
    
    private void DisplayCommandExecutionMetrics(MeasuredResult<int> profiledResult)
    {
        commandInfoDisplayService.DisplayExecutionMetrics(profiledResult);
    }
    
    protected abstract Task<int> ExecuteCommandAsync(CommandContext context, TSettings settings);
}
