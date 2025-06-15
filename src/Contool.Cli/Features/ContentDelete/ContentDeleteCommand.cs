using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Cli.Features.ContentDelete;

public class ContentDeleteCommand(
    ICommandHandler<Core.Features.ContentDelete.ContentDeleteCommand> handler) : AsyncCommand<ContentDeleteCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type <CONTENT_TYPE_ID>")]
        [Description("Content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-a|--apply")]
        [Description("Whether to perform the delete (omit for dry run).")]
        public bool Apply { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentDelete.ContentDeleteCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            Apply = settings.Apply
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
