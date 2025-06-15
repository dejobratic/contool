using Spectre.Console.Cli;
using System.ComponentModel;

namespace Contool.Cli.Features;

public class SettingsBase : CommandSettings
{
    [CommandOption("-s|--space <SPACE_ID>")]
    [Description("Contentful space ID.")]
    public string? SpaceId { get; init; }

    [CommandOption("-e|--environment <ENV_ID>")]
    [Description("Contentful environment ID.")]
    public string? EnvironmentId { get; init; }
}
