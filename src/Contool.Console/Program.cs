using Contool.Console;
using Contool.Console.Commands.Content;
using Contool.Console.Commands.Info;
using Contool.Console.Commands.Login;
using Contool.Console.Commands.Logout;
using Contool.Console.Commands.Type;
using Contool.Console.Infrastructure;
using Spectre.Console.Cli;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TypeRegistrar = Contool.Console.Infrastructure.Utils.TypeRegistrar;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.OutputEncoding = Encoding.Unicode;
}

var services = Dependencies.BuildServiceCollection();

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName(AppInfo.Name);

    config.AddCommand<InfoCommand>("info")
        .WithDescription("Show Contentful profile information.");

    config.AddCommand<LoginCommand>("login")
        .WithDescription("Configure Contentful profile.");

    config.AddCommand<LogoutCommand>("logout")
        .WithDescription("Remove Contentful profile configuration.");

    config.AddBranch("content", branchConfig =>
    {
        branchConfig.SetDescription("Manage Contentful content entries using bulk operations.");

        branchConfig.AddCommand<ContentDownloadCommand>("download") // TODO: consider renaming to "export"
            .WithDescription("Download entries for a given content type.");

        branchConfig.AddCommand<ContentUploadCommand>("upload") // TODO: consider renaming to "import"
            .WithDescription("Upload entries for a given content type.");

        branchConfig.AddCommand<ContentDeleteCommand>("delete")
            .WithDescription("Delete entries for a given content type.");

        branchConfig.AddCommand<ContentPublishCommand>("publish")
            .WithDescription("Publish entries for a given content type.");

        branchConfig.AddCommand<ContentUnpublishCommand>("unpublish")
            .WithDescription("Unpublish entries for a given content type.");
    });

    config.AddBranch("type", branchConfig =>
    {
        branchConfig.SetDescription("Manage Contentful content types (models).");

        branchConfig.AddCommand<TypeCloneCommand>("clone")
            .WithDescription("Clone a content type.");

        branchConfig.AddCommand<TypeDeleteCommand>("delete")
            .WithDescription("Delete a content type.");
    });
});

return await app.RunAsync(args);