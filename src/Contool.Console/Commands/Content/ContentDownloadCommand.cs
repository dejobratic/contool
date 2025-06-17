using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentDownloadCommand(
    ICommandHandler<Core.Features.ContentDownload.ContentDownloadCommand> handler) : AsyncCommand<ContentDownloadCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type <CONTENT_TYPE_ID>")]
        [Description("The content type ID to download.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-o|--output-path <OUTPUT_PATH>")]
        [Description("The output folder path.")]
        [Required]
        public string OutputPath { get; init; } = default!;

        [CommandOption("-f|--format <FORMAT>")]
        [Description("The output file format (CSV, JSON).")]
        [Required]
        public string OutputFormat { get; init; } = default!;
    }

    public override Spectre.Console.ValidationResult Validate(CommandContext context, Settings settings)
    {
        return base.Validate(context, settings);
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentDownload.ContentDownloadCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            OutputPath = settings.OutputPath,
            OutputFormat = settings.OutputFormat
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
