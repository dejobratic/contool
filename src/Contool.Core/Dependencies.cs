using Contentful.Core.Configuration;
using Contool.Core.Features;
using Contool.Core.Features.ContentDelete;
using Contool.Core.Features.ContentDownload;
using Contool.Core.Features.ContentPublish;
using Contool.Core.Features.ContentUnpublish;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Features.TypeClone;
using Contool.Core.Features.TypeDelete;
using Contool.Core.Infrastructure.Contentful.Options;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contool.Core;

public static class Dependencies
{
    public static IServiceCollection AddContoolDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddInfrastructure(configuration)
            .AddCommands();
    }

    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<ResiliencyOptions>(configuration.GetSection("ResiliencyOptions"))

            // Contentful
            .Configure<ContentfulOptions>(configuration.GetSection("ContentfulOptions"))
            .AddHttpClient()
            .AddSingleton<IContentfulManagementClientAdapterFactory, ContentfulManagementClientAdapterFactory>()
            .AddSingleton<IContentfulLoginServiceBuilder, ContentfulLoginServiceBuilder>()
            .AddSingleton<IContentfulServiceBuilder, ContentfulServiceBuilder>()

            // Contentful services
            .AddSingleton<IContentfulEntryOperationServiceFactory, ContentfulEntryOperationServiceFactory>()
            .AddSingleton<IContentDownloader, ContentDownloader>()
            .AddSingleton<IContentUploader, ContentUploader>()
            .AddSingleton<IContentPublisher, ContentPublisher>()
            .AddSingleton<IContentUnpublisher, ContentUnpublisher>()
            .AddSingleton<IContentDeleter, ContentDeleter>()
            .AddSingleton<IContentCloner, ContentCloner>()

            // Serialization/Deserialization
            .AddSingleton<IContentEntrySerializerFactory, ContentEntrySerializerFactory>()
            .AddSingleton<IContentEntryDeserializerFactory, ContentEntryDeserializerFactory>()

            // IO
            .AddSingleton<IOutputWriterFactory, OutputWriterFactory>()
            .AddSingleton<IOutputWriter, CsvOutputWriter>()
            .AddSingleton<IOutputWriter, ExcelOutputWriter>()
            .AddSingleton<IOutputWriter, JsonOutputWriter>()
            .AddSingleton<IInputReaderFactory, InputReaderFactory>()
            .AddSingleton<IInputReader, CsvInputReader>()
            .AddSingleton<IInputReader, ExcelInputReader>()
            .AddSingleton<IInputReader, JsonInputReader>()

            // Utils
            .AddSingleton<IOperationTracker, OperationTracker>()
            .AddSingleton<IBatchProcessor, BatchProcessor>()
            .AddSingleton<IRuntimeContext, RuntimeContext>();
    }

    private static IServiceCollection AddCommands(this IServiceCollection services)
    {
        return services
            // content
            .AddSingleton<ICommandHandler<ContentDownloadCommand>, ContentDownloadCommandHandler>()
            .AddSingleton<ICommandHandler<ContentUploadCommand>, ContentUploadCommandHandler>()
            .AddSingleton<ICommandHandler<ContentPublishCommand>, ContentPublishCommandHandler>()
            .AddSingleton<ICommandHandler<ContentUnpublishCommand>, ContentUnpublishCommandHandler>()
            .AddSingleton<ICommandHandler<ContentDeleteCommand>, ContentDeleteCommandHandler>()

            // types
            .AddSingleton<ICommandHandler<TypeCloneCommand>, TypeCloneCommandHandler>()
            .AddSingleton<ICommandHandler<TypeDeleteCommand>, TypeDeleteCommandHandler>();
    }
}
