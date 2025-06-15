using Contool.Cli;
using Contool.Cli.Features.ContentDelete;
using Contool.Cli.Features.ContentDownload;
using Contool.Cli.Features.ContentUpload;
using Contool.Cli.Features.TypeClone;
using Contool.Cli.Features.TypeDelete;
using Contool.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.Runtime.InteropServices;
using System.Text;
using TypeRegistrar = Contool.Cli.Infrastructure.Utils.TypeRegistrar;

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
    config.SetApplicationName("contool");

    config.AddBranch("content", branchConfig =>
    {
        branchConfig.AddCommand<ContentDownloadCommand>("download")
            .WithDescription("Download entries for a given content type.");

        branchConfig.AddCommand<ContentUploadCommand>("upload")
            .WithDescription("Upload entries for a given content type.");

        branchConfig.AddCommand<ContentDeleteCommand>("delete")
            .WithDescription("Delete entries for a given content type.");
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
        .AddCliDependencies();
}

static IConfigurationRoot BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();
}