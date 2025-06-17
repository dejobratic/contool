using Contool.Console;
using Contool.Console.Commands.Content;
using Contool.Console.Commands.Info;
using Contool.Console.Commands.Login;
using Contool.Console.Commands.Type;
using Contool.Console.Infrastructure;
using Contool.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.Runtime.InteropServices;
using System.Text;
using TypeRegistrar = Contool.Console.Infrastructure.Utils.TypeRegistrar;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.OutputEncoding = Encoding.Unicode;
}

var configuration = BuildConfiguration();

var services = BuildServiceCollection(configuration);

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName(AppInfo.Name);

    config.AddCommand<InfoCommand>("info")
        .WithDescription("Show information about the current space and environment.");

    config.AddCommand<LoginCommand>("login")
        .WithDescription("Configure Contentful profile.");

    config.AddBranch("content", branchConfig =>
    {
        branchConfig.AddCommand<ContentDownloadCommand>("download") // TODO: consider renaming to "export"
            .WithDescription("Download entries for a given content type.");

        branchConfig.AddCommand<ContentUploadCommand>("upload") // TODO: consider renaming to "import"
            .WithDescription("Upload entries for a given content type.");

        branchConfig.AddCommand<ContentDeleteCommand>("delete")
            .WithDescription("Delete entries for a given content type.");

        branchConfig.AddCommand<ContentPublishCommand>("publish")
            .WithDescription("Publish entries for a given content type.");
    });

    config.AddBranch("type", branchConfig =>
    {
        branchConfig.AddCommand<TypeCloneCommand>("clone")
            .WithDescription("Clone a content type.");

        branchConfig.AddCommand<TypeDeleteCommand>("delete")
            .WithDescription("Delete a content type.");
    });
});

return await app.RunAsync(args);

static IServiceCollection BuildServiceCollection(IConfiguration configuration)
{
    return new ServiceCollection()
        .AddContoolDependencies(configuration)
        .AddConsoleDependencies();
}

static IConfigurationRoot BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();
}