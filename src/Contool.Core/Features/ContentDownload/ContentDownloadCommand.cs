using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Features.ContentDownload;

public class ContentDownloadCommand : CommandBase
{
    public string ContentTypeId { get; init; } = null!;

    public string OutputPath { get; init; } = null!;

    public string OutputFormat { get; init; } = null!;
}

public class ContentDownloadCommandHandler(
    IContentEntrySerializerFactory serializerFactory,
    IContentfulServiceBuilder contentfulServiceBuilder,
    IOutputWriterFactory outputWriterFactory,
    IContentDownloader contentDownloader) : ICommandHandler<ContentDownloadCommand>
{
    public async Task HandleAsync(ContentDownloadCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var serializer = await serializerFactory.CreateAsync(
            command.ContentTypeId, contentfulService, cancellationToken);

        var output = GetEntriesForDownload(
            command, contentfulService, serializer, cancellationToken);

        var outputWriter = outputWriterFactory.Create(
            output.DataSource);

        await DownloadAsync(
            command, output, outputWriter, cancellationToken);
    }

    private static OutputContent GetEntriesForDownload(
        ContentDownloadCommand command,
        IContentfulService contentfulService,
        IContentEntrySerializer serializer,
        CancellationToken cancellationToken)
    {
        var entriesToDownload = GetEntriesToDownload(
            command.ContentTypeId, contentfulService, serializer, cancellationToken);

        return new OutputContent(
            path: command.OutputPath,
            name: command.ContentTypeId,
            type: command.OutputFormat,
            content: entriesToDownload);
    }

    private static AsyncEnumerableWithTotal<dynamic> GetEntriesToDownload(
        string contentTypeId,
        IContentfulService contentfulService,
        IContentEntrySerializer serializer,
        CancellationToken cancellationToken)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId, cancellationToken: cancellationToken);

        return new AsyncEnumerableWithTotal<dynamic>(
            source: entries.Select(entry => serializer.Serialize(entry)),
            getTotal: () => entries.Total);
    }

    private async Task DownloadAsync(
        ContentDownloadCommand command,
        OutputContent output,
        IOutputWriter outputWriter,
        CancellationToken cancellationToken)
    {
        var input = CreateContentDownloaderInput(
            command, output, outputWriter);
        
        await contentDownloader.DownloadAsync(
            input, cancellationToken);
    }

    private static ContentDownloaderInput CreateContentDownloaderInput(
        ContentDownloadCommand command,
        OutputContent output,
        IOutputWriter outputWriter)
    {
        return new ContentDownloaderInput
        {
            ContentTypeId = command.ContentTypeId,
            Output = output,
            OutputWriter = outputWriter,
        };
    }
}

