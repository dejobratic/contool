using Contool.Console.Infrastructure.Utils;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Contool.Console.Commands;

public class SettingsBase : CommandSettings
{
    [Secret]
    [CommandOption("--management-token <TOKEN>")]
    [Description($"Your Contentful Management API (CMA) token. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/references/authentication/[/]")]
    public string? ContentManagementToken { get; set; }

    [Secret]
    [CommandOption("--delivery-token <TOKEN>")]
    [Description("Your Contentful Content Delivery API (CDA) token. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/references/authentication/[/]")]
    public string? ContentDeliveryToken { get; set; }

    [Secret]
    [CommandOption("--preview-token <TOKEN>")]
    [Description("Your Contentful Content Preview API token. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/references/authentication/[/]")]
    public string? ContentPreviewToken { get; set; }

    [CommandOption("-s|--space-id <ID>")]
    [Description("The Contentful space identifier. See [italic LightGoldenrod2]https://www.contentful.com/help/spaces-and-organizations/[/]")]
    public string? SpaceId { get; set; }

    [CommandOption("-e|--environment-id <ID>")]
    [Description("The Contentful environment identifier. See [italic LightGoldenrod2]https://www.contentful.com/developers/docs/concepts/multiple-environments/[/]")]
    public string? EnvironmentId { get; set; }
}