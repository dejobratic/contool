using Contool.Commands;
using Contool.Contentful.Services;
using Contool.Services;
using Contentful.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http.Headers;

var stopwatch = Stopwatch.StartNew();
stopwatch.Start();

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var serviceProvider = new ServiceCollection()
    .Configure<ContentfulOptions>(configuration.GetSection("ContentfulOptions"))
    .AddHttpClient()
    .AddSingleton<IContentfulManagementClientAdapterFactory, ContentfulManagementClientAdapterFactory>()
    .AddSingleton<Func<IContentfulManagementClientAdapter, IContentfulManagementClientAdapter>>(sp => 
        adapter => new ContentfulManagementClientAdapterResiliencyDecorator(adapter))
    .AddSingleton<IContentfulServiceBuilder, ContentfulServiceBuilder>()
    .AddSingleton<IContentEntrySerializerFactory, ContentEntrySerializerFactory>()
    .AddSingleton<IContentDownloader, ContentDownloader>()
    .AddSingleton<IOutputWriterFactory, OutputWriterFactory>()
    .AddSingleton<IOutputWriter, CsvOutputWriter>()
    .AddSingleton<IOutputWriter, JsonOutputWriter>()
    .AddSingleton<IContentEntryDeserializerFactory, ContentEntryDeserializerFactory>()
    .AddSingleton<IContentUploader, ContentUploader>()
    .AddSingleton<IInputReaderFactory, InputReaderFactory>()
    .AddSingleton<IInputReader, CsvInputReader>()
    .AddSingleton<IContentCloner, ContentCloner>()
    //.AddSingleton<IInputReader, JsonInputReader>()
    .AddSingleton<ContentDownloadCommandHandler>()
    .AddSingleton<ContentUploadCommandHandler>()
    .AddSingleton<ContentPublishCommandHandler>()
    .AddSingleton<ContentDeleteCommandHandler>()
    .AddSingleton<TypeCloneCommandHandler>()
    .AddSingleton<TypeDeleteCommandHandler>()
    .BuildServiceProvider();

var downloadCommand = new ContentDownloadCommand
{
    ContentTypeId = "templateTranslation",
    EnvironmentId = "master",
    OutputPath = @"C:\Users\dejanbratic\Desktop\contool-playground",
    OutputFormat = "csv",
};

var downloadCommandHanlder = serviceProvider.GetRequiredService<ContentDownloadCommandHandler>();

await downloadCommandHanlder.HandleAsync(downloadCommand);

var uploadCommand = new ContentUploadCommand
{
    ContentTypeId = "templateTranslation",
    EnvironmentId = "production",
    InputPath = @"C:\Users\dejanbratic\Desktop\contool-playground\templateTranslation.csv",
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

var deleteCommand = new TypeDeleteCommand
{
    ContentTypeId = "templateTranslation",
    EnvironmentId = "production",
};

var deleteCommandHandler = serviceProvider.GetRequiredService<TypeDeleteCommandHandler>();

//await deleteCommandHandler.HandleAsync(deleteCommand);

// what about cloning a referenced content type?
var cloneCommand = new TypeCloneCommand
{
    ContentTypeId = "templateTranslation",
    EnvironmentId = "master",
    TargetEnvironmentId = "production",
    ShouldPublish = true,
};

var cloneCommandHandler = serviceProvider.GetRequiredService<TypeCloneCommandHandler>();

await cloneCommandHandler.HandleAsync(cloneCommand);

AnsiConsole.Markup($"[underline red]Hello[/] World! Total time: {stopwatch.ElapsedMilliseconds} ms");