using Contool.Commands;
using Contool.Contentful.Services;
using Contool.Services;
using Contentful.Core;
using Contentful.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;


var options = new ContentfulOptions
{

};

var serviceProvider = new ServiceCollection()
    .AddSingleton(options)
    .AddHttpClient()
    .AddSingleton<IContentfulManagementClient>(ctx =>
        new ContentfulManagementClient(
            ctx.GetRequiredService<IHttpClientFactory>().CreateClient(),
            ctx.GetRequiredService<ContentfulOptions>()))
    .AddSingleton<IContentfulService, ContentfulService>()
    .AddSingleton<IContentEntrySerializerFactory, ContentEntrySerializerFactory>()
    .AddSingleton<IContentDownloader, ContentDownloader>()
    .AddSingleton<IOutputWriterFactory, OutputWriterFactory>()
    .AddSingleton<IOutputWriter, CsvOutputWriter>()
    .AddSingleton<IOutputWriter, JsonOutputWriter>()
    .AddSingleton<IContentEntryDeserializerFactory, ContentEntryDeserializerFactory>()
    .AddSingleton<IContentUploader, ContentUploader>()
    .AddSingleton<IInputReaderFactory, InputReaderFactory>()
    .AddSingleton<IInputReader, CsvInputReader>()
    //.AddSingleton<IInputReader, JsonInputReader>()
    .AddSingleton<ContentDownloadCommandHandler>()
    .AddSingleton<ContentUploadCommandHandler>()
    .BuildServiceProvider();

var downloadCommand = new ContentDownloadCommand
{
    ContentTypeId = "brand",
    OutputPath = @"C:\Users\dejanbratic",
    OutputFormat = "csv",
};

var downloadCommandHanlder = serviceProvider.GetRequiredService<ContentDownloadCommandHandler>();

await downloadCommandHanlder.HandleAsync(downloadCommand);


var uploadCommand = new ContentUploadCommand
{
    ContentTypeId = "brand",
    InputPath = @"C:\Users\dejanbratic\brand.csv",
    ShouldPublish = true,
};

var uploadCommandHandler = serviceProvider.GetRequiredService<ContentUploadCommandHandler>();

await uploadCommandHandler.HandleAsync(uploadCommand);

AnsiConsole.Markup("[underline red]Hello[/] World!");