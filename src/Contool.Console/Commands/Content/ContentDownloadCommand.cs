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
using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Console.Commands.Content;

public class ContentDownloadCommand(
    ICommandHandler<Core.Features.ContentDownload.ContentDownloadCommand> handler,
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandInfoDisplayService commandInfoDisplayService,
    IErrorDisplayService errorDisplayService)
    : LoggedInCommandBase<ContentDownloadCommand.Settings>(runtimeContext, contentfulServiceBuilder, commandInfoDisplayService, errorDisplayService)
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("The Contentful content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = null!;

        [CommandOption("-o|--output-path <PATH>")]
        [Description("The output folder path. If not specified, uses the current working directory.")]
        public string? OutputPath { get; init; }

        [CommandOption("-f|--output-format <FORMAT>")]
        [Description("The output file format (EXCEL, CSV, JSON).")]
        public string? OutputFormat { get; init; }
    }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentDownload.ContentDownloadCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            OutputPath = settings.OutputPath ?? Environment.CurrentDirectory,
            OutputFormat = settings.OutputFormat ?? DataSource.Csv.Name,
        };

        await handler.HandleAsync(command);
        return CommandResult.Success;
    }
}
