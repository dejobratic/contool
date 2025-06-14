using Contentful.Core.Configuration;
using Contool.Core.Contentful.Options;
using Contool.Core.Contentful.Services;
using Contool.Core.Features;
using Contool.Core.Features.EntryDelete;
using Contool.Core.Features.EntryDownload;
using Contool.Core.Features.EntryPublish;
using Contool.Core.Features.EntryUpload;
using Contool.Core.Features.TypeClone;
using Contool.Core.Features.TypeDelete;
using Contool.Core.Infrastructure.IO.Input;
using Contool.Core.Infrastructure.IO.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Contool.Core;

public static class Dependencies
{
    public static IServiceCollection AddContoolDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<ContentfulOptions>(configuration.GetSection("ContentfulOptions"))
            .Configure<ResiliencyOptions>(configuration.GetSection("ResiliencyOptions"))
            .AddHttpClient()
            .AddSingleton<IContentfulManagementClientAdapterFactory, ContentfulManagementClientAdapterFactory>()
            .AddSingleton<Func<IContentfulManagementClientAdapter, IContentfulManagementClientAdapter>>(sp =>
            {
                var resiliencyOptions = sp.GetRequiredService<IOptions<ResiliencyOptions>>();
                return adapter => new ContentfulManagementClientAdapterResiliencyDecorator(resiliencyOptions, adapter);
            })
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
            .AddSingleton<IInputReader, JsonInputReader>()
            .AddSingleton<ICommandHandler<ContentDownloadCommand>, ContentDownloadCommandHandler>()
            .AddSingleton<ICommandHandler<ContentUploadCommand>, ContentUploadCommandHandler>()
            .AddSingleton<ICommandHandler<ContentPublishCommand>, ContentPublishCommandHandler>()
            .AddSingleton<ICommandHandler<ContentDeleteCommand>, ContentDeleteCommandHandler>()
            .AddSingleton<ICommandHandler<TypeCloneCommand>, TypeCloneCommandHandler>()
            .AddSingleton<ICommandHandler<TypeDeleteCommand>, TypeDeleteCommandHandler>();
    }
}
