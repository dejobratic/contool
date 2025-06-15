using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Cli.Features.ContentPublish;

public class ContentPublishCommand(
    ICommandHandler<Core.Features.ContentPublish.ContentPublishCommand> handler)
    : AsyncCommand<ContentPublishCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type <CONTENT_TYPE_ID>")]
        [Description("The ID of the content type to publish.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
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
