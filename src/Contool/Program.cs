using Contool.Core;
using Contool.Core.Features.EntryDelete;
using Contool.Core.Features.EntryDownload;
using Contool.Core.Features.EntryPublish;
using Contool.Core.Features.EntryUpload;
using Contool.Core.Features.TypeClone;
using Contool.Core.Features.TypeDelete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();
stopwatch.Start();

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var serviceProvider = new ServiceCollection()
    .AddContoolDependencies(configuration)
    .BuildServiceProvider();

var downloadCommand = new ContentDownloadCommand
{
    ContentTypeId = "brand",
    EnvironmentId = "production",
    OutputPath = @"C:\Users\dejanbratic\Desktop\contool-playground",
    OutputFormat = "csv",
};

var downloadCommandHanlder = serviceProvider.GetRequiredService<ContentDownloadCommandHandler>();

//await downloadCommandHanlder.HandleAsync(downloadCommand);

var uploadCommand = new ContentUploadCommand
{
    ContentTypeId = "brand",
    EnvironmentId = "production",
    InputPath = @"C:\Users\dejanbratic\Desktop\contool-playground\brand_1000.csv",
    ShouldPublish = true,
};

var uploadCommandHandler = serviceProvider.GetRequiredService<ContentUploadCommandHandler>();

//await uploadCommandHandler.HandleAsync(uploadCommand);

var publishCommand = new ContentPublishCommand
{
    ContentTypeId = "templateTranslation",
    EnvironmentId = "production",
};

var publishCommandHandler = serviceProvider.GetRequiredService<ContentPublishCommandHandler>();

//await publishCommandHandler.HandleAsync(publishCommand);

var typeDeleteCommand = new TypeDeleteCommand
{
    ContentTypeId = "brand",
    EnvironmentId = "production",
    Force = true,
};

var typeDeleteCommandHandler = serviceProvider.GetRequiredService<TypeDeleteCommandHandler>();

try
{

    //await typeDeleteCommandHandler.HandleAsync(deleteCommand);
}
catch (InvalidOperationException ex)
{
    AnsiConsole.MarkupLine($"[red]Type delete error:[/] {ex.Message}");
}

var entryDeleteCommand = new ContentDeleteCommand
{
    ContentTypeId = "brand",
    EnvironmentId = "production",
};

var entryDeleteCommandHandler = serviceProvider.GetRequiredService<ContentDeleteCommandHandler>();

try
{
    await entryDeleteCommandHandler.HandleAsync(entryDeleteCommand);
}
catch (InvalidOperationException ex)
{
    AnsiConsole.MarkupLine($"[red]Entry delete error:[/] {ex.Message}");
}

var cloneCommand = new TypeCloneCommand
{
    ContentTypeId = "templateTranslation",
    EnvironmentId = "master",
    TargetEnvironmentId = "production",
    ShouldPublish = true,
};

var cloneCommandHandler = serviceProvider.GetRequiredService<TypeCloneCommandHandler>();

try
{
    //await cloneCommandHandler.HandleAsync(cloneCommand);
}
catch (InvalidOperationException ex)
{
    AnsiConsole.MarkupLine($"[red]Type clone error:[/] {ex.Message}");
}

AnsiConsole.Markup($"[underline red]Hello[/] World! Total time: {stopwatch.ElapsedMilliseconds} ms");