using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Type;

public class TypeCloneCommand(
    ICommandHandler<Core.Features.TypeClone.TypeCloneCommand> handler) : AsyncCommand<TypeCloneCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-t|--target-environment <TARGET_ENVIRONMENT_ID>")]
        [Description("Target environment ID where the type will be cloned.")]
        [Required]
        public string TargetEnvironmentId { get; init; } = default!;

        [CommandOption("-c|--content-type <CONTENT_TYPE_ID>")]
        [Description("Content type ID to clone.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-p|--publish")]
        [Description("Whether to publish the cloned type.")]
        public bool Publish { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.TypeClone.TypeCloneCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            TargetEnvironmentId = settings.TargetEnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            ShouldPublish = settings.Publish
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
