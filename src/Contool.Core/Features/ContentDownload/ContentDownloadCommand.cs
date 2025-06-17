using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Extensions;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Output;
using Contool.Core.Infrastructure.Utils;
using Microsoft.Extensions.Logging;

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
    IProgressReporter progressReporter,
    ILogger<ContentDownloadCommandHandler> logger) : ICommandHandler<ContentDownloadCommand>
{
    public async Task HandleAsync(ContentDownloadCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var serializer = await serializerFactory.CreateAsync(
            command.ContentTypeId, contentfulService, cancellationToken);

        var output = await DownloadAsync(
            command, contentfulService, serializer, cancellationToken);

        var outputWriter = outputWriterFactory.Create(
            output.DataSource);

        await outputWriter.SaveAsync(
            output.FullPath, output.Content, cancellationToken);

        logger.LogInformation(
            "{Total} {ContentTypeId} entries downloaded to {OutputPath}.", output.Content.Total, command.ContentTypeId, output.FullPath);
    }

    public async Task<OutputContent> DownloadAsync(
        ContentDownloadCommand command,
        IContentfulService contentfulService,
        IContentEntrySerializer serializer,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // TODO: remove ??

        var entriesToDownload = GetEntriesToDownload(
            command.ContentTypeId, contentfulService, serializer, cancellationToken);

        return new OutputContent( // TODO: remove this model, as the content downloader service is deleted
            path: command.OutputPath,
            name: command.ContentTypeId,
            type: command.OutputFormat,
            content: entriesToDownload);
    }

    private AsyncEnumerableWithTotal<dynamic> GetEntriesToDownload(
        string contentTypeId,
        IContentfulService contentfulService,
        IContentEntrySerializer serializer,
        CancellationToken cancellationToken)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId, cancellationToken: cancellationToken);

        return new AsyncEnumerableWithTotal<dynamic>(
            source: GetEntriesToSerialize(entries, serializer),
            getTotal: () => entries.Total,
            progressReporter.WithOperationName("Downloading")); // TODO: move this out of command handler
    }

    public static async IAsyncEnumerable<dynamic> GetEntriesToSerialize(
        IAsyncEnumerable<Entry<dynamic>> entries,
        IContentEntrySerializer serializer)
    {
        await foreach (var entry in entries)
            yield return serializer.Serialize(entry);
    }
}

