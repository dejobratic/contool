using Contool.Core.Features;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contool.Console.Commands.Content;

public class ContentUploadCommand(
    ICommandHandler<Core.Features.ContentUpload.ContentUploadCommand> handler)
    : CommandBase<ContentUploadCommand.Settings>
{
    public class Settings : SettingsBase
    {
        [CommandOption("-c|--content-type-id <ID>")]
        [Description("Content type ID.")]
        [Required]
        public string ContentTypeId { get; init; } = default!;

        [CommandOption("-i|--input-path <PATH>")]
        [Description("Path to input file (EXCEL, CSV, JSON).")]
        [Required]
        public string InputPath { get; init; } = default!;

        [CommandOption("-p|--publish")]
        [Description("Whether to publish the entries after upload (omit for draft).")]
        public bool Publish { get; init; }
    }

    public override Spectre.Console.ValidationResult Validate(CommandContext context, Settings settings)
    {
        return base.Validate(context, settings);
    }

    protected override async Task<int> ExecuteInternalAsync(CommandContext context, Settings settings)
    {
        var command = new Core.Features.ContentUpload.ContentUploadCommand
        {
            SpaceId = settings.SpaceId,
            EnvironmentId = settings.EnvironmentId,
            ContentTypeId = settings.ContentTypeId,
            InputPath = settings.InputPath,
            ShouldPublish = settings.Publish
        };

        await handler.HandleAsync(command);
        return 0;
    }
}
