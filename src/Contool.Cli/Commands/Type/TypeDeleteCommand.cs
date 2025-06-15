using Contool.Cli.Commands;
using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Cli.Commands.Type;

public class TypeDeleteCommand(
    ICommandHandler<Core.Features.TypeDelete.TypeDeleteCommand> handler) : AsyncCommand<TypeDeleteCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type <CONTENT_TYPE_ID>")]
        [Description("ID of the content type to delete.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-f|--force")]
        [Description("Force deletion without confirmation.")]
        public bool Force { get; init; } = false;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
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
