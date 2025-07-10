using Contool.Console.Infrastructure.UI;
using Contool.Console.Infrastructure.UI.Services;
using Contool.Console.Infrastructure.Utils;
using Contool.Console.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Spectre.Console.Cli;

namespace Contool.Console.Commands.Info;

public sealed class InfoCommand(
    IContentfulInfoDisplayService contentfulInfoDisplayService,
    IRuntimeContext runtimeContext,
    IContentfulLoginServiceBuilder contentfulServiceBuilder,
    ICommandInfoDisplayService commandInfoDisplayService,
    IErrorDisplayService errorDisplayService)
    : LoggedInCommandBase<InfoCommand.Settings>(runtimeContext, contentfulServiceBuilder, commandInfoDisplayService, errorDisplayService)
{
    private readonly IContentfulLoginServiceBuilder _contentfulServiceBuilder = contentfulServiceBuilder;

    public class Settings : SettingsBase { }

    protected override async Task<int> ExecuteLoggedInCommandAsync(CommandContext context, Settings settings)
    {
        var contentfulService = _contentfulServiceBuilder
            .WithSpaceId(settings.SpaceId)
            .WithEnvironmentId(settings.EnvironmentId)
            .Build();

        await contentfulInfoDisplayService.DisplayInfoAsync(contentfulService);

        return CommandResult.Success;
    }
}