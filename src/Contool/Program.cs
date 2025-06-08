using Contool.Commands;
using Contool.Contentful.Services;
using Contool.Services;
using Contentful.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

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
    .AddSingleton<ContentPublishCommand>()
    .AddSingleton<TypeCloneCommandHandler>()
    .BuildServiceProvider();

var downloadCommand = new ContentDownloadCommand
{
    ContentTypeId = "brand",
    EnvironmentId = "master",
    OutputPath = @"C:\Users\dejanbratic\Desktop\contool-playground",
    OutputFormat = "csv",
};

var downloadCommandHanlder = serviceProvider.GetRequiredService<ContentDownloadCommandHandler>();

await downloadCommandHanlder.HandleAsync(downloadCommand);


var uploadCommand = new ContentUploadCommand
{
    ContentTypeId = "brand",
    EnvironmentId = "master",
    InputPath = @"C:\Users\dejanbratic\Desktop\contool-playground\brand.csv",
    ShouldPublish = true,
};

var uploadCommandHandler = serviceProvider.GetRequiredService<ContentUploadCommandHandler>();

await uploadCommandHandler.HandleAsync(uploadCommand);

var cloneCommand = new TypeCloneCommand
{
    ContentTypeId = "brand",
    EnvironmentId = "master",
    TargetEnvironmentId = "production",
    ShouldPublish = true,
};

var cloneCommandHandler = serviceProvider.GetRequiredService<TypeCloneCommandHandler>();

await cloneCommandHandler.HandleAsync(cloneCommand);

AnsiConsole.Markup($"[underline red]Hello[/] World! Total time: {stopwatch.ElapsedMilliseconds} ms");