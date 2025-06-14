using Contool.Core.Features;
using Contool.Core.Features.EntryDownload;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Cli;

public class EntriesDownloadCommand(
    ICommandHandler<ContentDownloadCommand> handler) : AsyncCommand<EntriesDownloadCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-s|--space <SPACE_ID>")]
        [Description("The Contentful space ID.")]
        public string? SpaceId { get; init; }

        [CommandOption("-e|--environment <ENV_ID>")]
        [Description("The Contentful environment ID.")]
        public string? EnvironmentId { get; init; }

        [CommandOption("-c|--content-type <CONTENT_TYPE_ID>")]
        [Description("The content type ID to download.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-o|--output <OUTPUT_PATH>")]
        [Description("The output file path.")]
        [Required]
        public string OutputPath { get; init; } = default!;

        [CommandOption("-f|--format <FORMAT>")]
        [Description("The output format (CSV, JSON).")]
        [Required]
        public string OutputFormat { get; init; } = default!;
    }

    public override Spectre.Console.ValidationResult Validate(CommandContext context, Settings settings)
    {
        return base.Validate(context, settings);
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var command = new ContentDownloadCommand
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
