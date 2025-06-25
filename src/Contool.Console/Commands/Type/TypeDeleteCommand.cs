using Contool.Core.Features;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Type;

public class TypeDeleteCommand(
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandHandler<Core.Features.TypeDelete.TypeDeleteCommand> handler)
    : LoggedInCommandBase<TypeDeleteCommand.Settings>(runtimeContext, contentfulServiceBuilder)
{
    public class Settings : WriteSettingsBase
    {
        [CommandOption("-c|--content-type <ID>")]
        [Description("ID of the content type to delete.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-f|--force")]
        [Description("Force deletion of the content type even if it contains entries.")]
        public bool Force { get; init; } = false;
    }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.TypeDelete.TypeDeleteCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            Force = settings.Force,
            ApplyChanges = settings.Apply,
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
