using Contool.Core.Features;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentUnpublishCommand(
    IRuntimeContext runtimeContext,
    ICommandHandler<Core.Features.ContentUnpublish.ContentUnpublishCommand> handler)
    : CommandBase<ContentUnpublishCommand.Settings>(runtimeContext)
{
    public class Settings : WriteSettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The Contentful content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentUnpublish.ContentUnpublishCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            ApplyChanges = settings.Apply,
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
