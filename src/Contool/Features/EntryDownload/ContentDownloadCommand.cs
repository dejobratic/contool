using Contool.Contentful.Extensions;
using Contool.Contentful.Services;
using Contool.Infrastructure.IO.Output;

namespace Contool.Features.EntryDownload;

internal class ContentDownloadCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public string OutputPath { get; init; } = default!;

    public string OutputFormat { get; init; } = default!;
}

internal class ContentDownloadCommandHandler(
    IContentEntrySerializerFactory serializerFactory,
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentDownloader contentDownloader,
    IOutputWriterFactory outputWriterFactory)
{
    public async Task HandleAsync(ContentDownloadCommand command, CancellationToken cancellationToken = default)
    {
        // moving content type validation to the command handler

        var contentfulService = contentfulServiceBuilder
            .Build(command.SpaceId, command.EnvironmentId);

        var serializer = await serializerFactory
            .CreateAsync(command.ContentTypeId, contentfulService, cancellationToken);

        var output = await contentDownloader
            .DownloadAsync(CreateContentDownloadRequest(command, serializer, contentfulService), cancellationToken);

        var outputWriter = outputWriterFactory.Create(output.DataSource);
        await outputWriter.SaveAsync(output, cancellationToken);
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

