using Contool.Contentful.Models;
using Contool.Contentful.Services;
using Contool.Services;

namespace Contool.Commands;

internal class ContentDownloadCommand
{
    public string ContentTypeId { get; init; } = default!;
    public string OutputPath { get; init; } = default!;
    public string OutputFormat { get; init; } = default!;
}

internal class ContentDownloadCommandHandler(
    IContentEntrySerializerFactory serializerFactory,
    IContentDownloader contentDownloader,
    IOutputWriterFactory outputWriterFactory)
{
    public async Task HandleAsync(ContentDownloadCommand command, CancellationToken cancellationToken = default)
    {
        var serializer = await serializerFactory
            .CreateAsync(command.ContentTypeId, cancellationToken);
        
        var output = await contentDownloader
            .DownloadAsync(CreateContentDownloadRequest(command, serializer), cancellationToken);
        
        var outputWriter = outputWriterFactory.Create(output.DataSource);
        await outputWriter.SaveAsync(output, cancellationToken);
    }

    private static ContentDownloadRequest CreateContentDownloadRequest(ContentDownloadCommand command, IContentEntrySerializer serializer)
    {
        return new ContentDownloadRequest
        {
            ContentTypeId = command.ContentTypeId,
            OutputPath = command.OutputPath,
            OutputFormat = command.OutputFormat,
            Serializer = serializer
        };
    }
}

