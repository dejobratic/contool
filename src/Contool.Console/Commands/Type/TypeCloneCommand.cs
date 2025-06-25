using Contool.Core.Features;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Type;

public class TypeCloneCommand(
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandHandler<Core.Features.TypeClone.TypeCloneCommand> handler)
    : LoggedInCommandBase<TypeCloneCommand.Settings>(runtimeContext, contentfulServiceBuilder)
{
    public class Settings : WriteSettingsBase
    {
        [CommandOption("-t|--target-environment-id <ID>")]
        [Description("Target environment ID where the type will be cloned. See [italic LightGoldenrod2]https://www.contentful.com/help/spaces-and-organizations/[/]")]
        [Required]
        public string TargetEnvironmentId { get; init; } = default!;

        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The Contentful content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-p|--publish")]
        [Description("Whether to publish the cloned type entries (omit for draft).")]
        public bool Publish { get; init; }
    }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.TypeClone.TypeCloneCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            TargetEnvironmentId = settings.TargetEnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            ShouldPublish = settings.Publish,
            ApplyChanges = settings.Apply,
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
