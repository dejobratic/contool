using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.Utils;
using Contool.Core.Features;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Contool.Console.Infrastructure.UI.Services;
using Contool.Console.Infrastructure.Utils.Models;

namespace Contool.Console.Commands.Content;

public class ContentUploadCommand(
    ICommandHandler<Core.Features.ContentUpload.ContentUploadCommand> handler,
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandInfoDisplayService commandInfoDisplayService,
    IErrorDisplayService errorDisplayService)
    : LoggedInCommandBase<ContentUploadCommand.Settings>(runtimeContext, contentfulServiceBuilder, commandInfoDisplayService, errorDisplayService)
{
    public class Settings : WriteSettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The Contentful content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = null!;

        [CommandOption("-i|--input-path <PATH>")]
        [Description("The input file path (EXCEL, CSV, JSON).")]
        [Required]
        public string InputPath { get; init; } = null!;

        [CommandOption("-p|--publish")]
        [Description("Upload Contentful entries as published (omit for draft).")]
        public bool Publish { get; init; }


        [CommandOption("--upload-only-valid")]
        [Description("Upload only valid entries, skipping invalid ones. If false, stops upload if any validation errors exist.")]
        public bool UploadOnlyValid { get; init; }
    }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentUpload.ContentUploadCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            InputPath = settings.InputPath,
            PublishUploaded = settings.Publish,
            UploadOnlyValid = settings.UploadOnlyValid,
        };

        await handler.HandleAsync(command);
        return CommandResult.Success;
    }
}
