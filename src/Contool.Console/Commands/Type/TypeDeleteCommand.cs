using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Type;

public class TypeDeleteCommand(
    ICommandHandler<Core.Features.TypeDelete.TypeDeleteCommand> handler)
    : CommandBase<TypeDeleteCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type <ID>")]
        [Description("ID of the content type to delete.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-f|--force")]
        [Description("Force deletion of the content type even if it contains entries.")]
        public bool Force { get; init; } = false;

        [CommandOption("-a|--apply")]
        [Description("Whether to perform the deletion process (omit for dry run).")]
        public bool Apply { get; init; }
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.TypeDelete.TypeDeleteCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            Force = settings.Force
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
