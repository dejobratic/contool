using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentDeleteCommand(
    ICommandHandler<Core.Features.ContentDelete.ContentDeleteCommand> handler)
    : CommandBase<ContentDeleteCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("Content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-a|--apply")]
        [Description("Whether to perform the delete (omit for dry run).")]
        public bool Apply { get; init; }
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentDelete.ContentDeleteCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            ApplyChanges = settings.Apply
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
