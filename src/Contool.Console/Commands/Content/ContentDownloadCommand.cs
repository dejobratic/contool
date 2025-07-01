using Contool.Core.Features;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentDownloadCommand(
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandHandler<Core.Features.ContentDownload.ContentDownloadCommand> handler)
    : LoggedInCommandBase<ContentDownloadCommand.Settings>(runtimeContext, contentfulServiceBuilder)
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The Contentful content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-o|--output-path <PATH>")]
        [Description("The output folder path. If not specified, uses the current working directory.")]
        public string? OutputPath { get; init; }

        [CommandOption("-f|--output-format <FORMAT>")]
        [Description("The output file format (EXCEL, CSV, JSON).")]
        [Required]
        public string OutputFormat { get; init; } = default!;
    }

    public override Spectre.Console.ValidationResult Validate(CommandContext context, Settings settings)
    {
        return base.Validate(context, settings);
    }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentDownload.ContentDownloadCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            OutputPath = settings.OutputPath ?? Environment.CurrentDirectory,
            OutputFormat = settings.OutputFormat
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
