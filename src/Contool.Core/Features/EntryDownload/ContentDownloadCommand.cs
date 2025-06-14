using Contool.Core.Contentful.Extensions;
using Contool.Core.Contentful.Services;
using Contool.Core.Infrastructure.IO.Output;

namespace Contool.Core.Features.EntryDownload;

public class ContentDownloadCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public string OutputPath { get; init; } = default!;

    public string OutputFormat { get; init; } = default!;
}

public class ContentDownloadCommandHandler(
    IContentEntrySerializerFactory serializerFactory,
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentDownloader contentDownloader,
    IOutputWriterFactory outputWriterFactory) : ICommandHandler<ContentDownloadCommand>
{
    public async Task HandleAsync(ContentDownloadCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder
            .Build(command.SpaceId, command.EnvironmentId);

        var serializer = await serializerFactory
            .CreateAsync(command.ContentTypeId, contentfulService, cancellationToken);

        var output = await contentDownloader
            .DownloadAsync(CreateContentDownloadRequest(command, serializer, contentfulService), cancellationToken);

        var outputWriter = outputWriterFactory
            .Create(output.DataSource);

        await outputWriter
            .SaveAsync(output.FullPath, output.Content, cancellationToken);
    }

    private static ContentDownloadRequest CreateContentDownloadRequest(
        ContentDownloadCommand command, IContentEntrySerializer serializer, IContentfulService contentfulService)
    {
        return new ContentDownloadRequest
        {
            ContentTypeId = command.ContentTypeId,
            OutputPath = command.OutputPath,
            OutputFormat = command.OutputFormat,
            ContentfulService = contentfulService,
            Serializer = serializer
        };
    }
}

