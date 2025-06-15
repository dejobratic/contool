using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Output;

namespace Contool.Core.Features.ContentDownload;

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
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var serializer = await serializerFactory.CreateAsync(
            command.ContentTypeId, contentfulService, cancellationToken);

        var contentDownloadRequest = CreateContentDownloadRequest(
            command, serializer, contentfulService);

        var output = await contentDownloader.DownloadAsync(
            contentDownloadRequest, cancellationToken);

        var outputWriter = outputWriterFactory.Create(
            output.DataSource);

        await outputWriter.SaveAsync(
            output.FullPath, output.Content, cancellationToken);
    }

    private static ContentDownloadRequest CreateContentDownloadRequest(
        ContentDownloadCommand command,
        IContentEntrySerializer serializer,
        IContentfulService contentfulService)
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

