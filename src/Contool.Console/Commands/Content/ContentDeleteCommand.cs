using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.Utils;
using Contool.Core.Features;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentDeleteCommand(
    ICommandHandler<Core.Features.ContentDelete.ContentDeleteCommand> handler,
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandInfoDisplayService commandInfoDisplayService,
    IErrorDisplayService errorDisplayService)
    : LoggedInCommandBase<ContentDeleteCommand.Settings>(runtimeContext, contentfulServiceBuilder, commandInfoDisplayService, errorDisplayService)
{
    public class Settings : WriteSettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The Contentful content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = null!;

        [CommandOption("-i|--include-archived")]
        [Description("Whether to include archived entries in the deletion process.")]
        public bool IncludeArchived { get; init; }
    }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentDelete.ContentDeleteCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
        };

        await handler.HandleAsync(command);
        return CommandResult.Success;
    }
}
