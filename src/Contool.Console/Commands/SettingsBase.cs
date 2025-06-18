using Spectre.Console.Cli;
using System.ComponentModel;

namespace Contool.Console.Commands;

public class SettingsBase : CommandSettings
{
    [CommandOption("-s|--space-id <ID>")]
    [Description("The Contentful space identifier. See [italic LightGoldenrod2]https://www.contentful.com/help/spaces-and-organizations/[/]")]
    public string? SpaceId { get; set; }

    [CommandOption("-e|--environment-id <ID>")]
    [Description("The Contentful environment identifier. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/concepts/multiple-environments/[/]")]
    public string? EnvironmentId { get; set; }
}
