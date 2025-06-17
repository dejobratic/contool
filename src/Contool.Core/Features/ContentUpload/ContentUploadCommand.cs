using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Input;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Contool.Core.Features.ContentUpload;

public class ContentUploadCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public string InputPath { get; init; } = default!;

    public bool ShouldPublish { get; init; }
}

public class ContentUploadCommandHandler(
    IInputReaderFactory inputReaderFactory,
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentEntryDeserializerFactory deserializerFactory,
    ILogger<ContentUploadCommandHandler> logger) : ICommandHandler<ContentUploadCommand>
{
    public async Task HandleAsync(ContentUploadCommand command, CancellationToken cancellationToken = default)
    {
        var inputReader = inputReaderFactory.Create(
            GetFileSource(command));

        var input = inputReader.ReadAsync(
            command.InputPath, cancellationToken);

        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var deserializer = await deserializerFactory.CreateAsync(
            command.ContentTypeId, contentfulService, cancellationToken);

        await UploadEntriesAsync(
            input, contentfulService, deserializer, command.ShouldPublish, cancellationToken);

        logger.LogInformation(
            "{Total} {ContentTypeId} entries uploaded.", input.Total, command.ContentTypeId);
    }

    private static DataSource GetFileSource(
        ContentUploadCommand command)
    {
        var fileExtension = Path.GetExtension(command.InputPath) ?? string.Empty;
        return DataSource.From(fileExtension);
    }

    public static async Task UploadEntriesAsync(
        IAsyncEnumerableWithTotal<dynamic> content,
        IContentfulService contentfulService,
        IContentEntryDeserializer deserializer,
        bool publish,
        CancellationToken cancellationToken)
    {
        var entriesForUploading = GetEntriesForUploading(
            content, deserializer, cancellationToken);

        await contentfulService.CreateOrUpdateEntriesAsync(
            entriesForUploading, publish, cancellationToken);
    }

    private static AsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesForUploading(
        IAsyncEnumerableWithTotal<dynamic> content,
        IContentEntryDeserializer deserializer,
        CancellationToken cancellationToken)
    {
        return new AsyncEnumerableWithTotal<Entry<dynamic>>(
            GetDeserializedentries(content, deserializer, cancellationToken),
            getTotal: () => content.Total);
    }

    private static async IAsyncEnumerable<Entry<dynamic>> GetDeserializedentries(
        IAsyncEnumerableWithTotal<dynamic> content,
        IContentEntryDeserializer deserializer,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var row in content.WithCancellation(cancellationToken))
            yield return deserializer.Deserialize(row);
    }
}
