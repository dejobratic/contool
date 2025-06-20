using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;

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

        await contentDownloader.DownloadAsync(
            command.ContentTypeId, output, outputWriter, cancellationToken);
    }

    public static OutputContent GetEntriesForDownload(
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
            source: GetEntriesToSerialize(entries, serializer),
            getTotal: () => entries.Total);
    }

    public static async IAsyncEnumerable<dynamic> GetEntriesToSerialize(
        IAsyncEnumerable<Entry<dynamic>> entries,
        IContentEntrySerializer serializer)
    {
        await foreach (var entry in entries)
            yield return serializer.Serialize(entry);
    }
}

