using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentPublishCommand(
    ICommandHandler<Core.Features.ContentPublish.ContentPublishCommand> handler)
    : CommandBase<ContentPublishCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The ID of the content type to publish.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentPublish.ContentPublishCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
