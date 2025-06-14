using Contool;
using Contool.Cli;
using Contool.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.Diagnostics;

using TypeRegistrar = Contool.Utils.TypeRegistrar;

var stopwatch = Stopwatch.StartNew();

var configuration = BuildConfiguration();

var services = BuildServiceCollection(configuration);

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("contool");

    config.AddBranch("content", branchConfig =>
    {
        branchConfig.AddCommand<EntriesDownloadCommand>("download")
            .WithDescription("Download entries for a given content type.");
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