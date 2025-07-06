using Contool.Core.Features;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentUploadCommand(
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandHandler<Core.Features.ContentUpload.ContentUploadCommand> handler)
    : LoggedInCommandBase<ContentUploadCommand.Settings>(runtimeContext, contentfulServiceBuilder)
{
    public class Settings : WriteSettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The Contentful content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-i|--input-path <PATH>")]
        [Description("The input file path (EXCEL, CSV, JSON).")]
        [Required]
        public string InputPath { get; init; } = default!;

        [CommandOption("-p|--publish")]
        [Description("Upload Contentful entries as published (omit for draft).")]
        public bool Publish { get; init; }
    }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentUpload.ContentUploadCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            InputPath = settings.InputPath,
            ShouldPublish = settings.Publish,
            ApplyChanges = settings.Apply,
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
