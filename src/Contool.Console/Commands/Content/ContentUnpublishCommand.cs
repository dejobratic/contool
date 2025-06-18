using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentUnpublishCommand(
    ICommandHandler<Core.Features.ContentUnpublish.ContentUnpublishCommand> handler)
    : CommandBase<ContentUnpublishCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The ID of the content type to unpublish.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentUnpublish.ContentUnpublishCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
