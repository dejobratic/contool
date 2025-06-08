using Contool.Contentful.Models;
using Contool.Contentful.Services;
using Contool.Models;
using Contool.Services;

namespace Contool.Commands;

internal class ContentUploadCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public string InputPath { get; init; } = default!;

    public bool ShouldPublish { get; init; }
}

internal class ContentUploadCommandHandler(
    IInputReaderFactory inputReaderFactory,
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentEntryDeserializerFactory deserializerFactory,
    IContentUploader contentUploader)
{
    public async Task HandleAsync(ContentUploadCommand command, CancellationToken cancellationToken = default)
    {
        var inputReader = inputReaderFactory.Create(GetFileSource(command));
        var input = await inputReader.ReadAsync(command.InputPath, cancellationToken);

        var contentfulService = contentfulServiceBuilder
            .WithSpaceId(command.SpaceId)
            .WithEnvironmentId(command.EnvironmentId)
            .Build();

        var deserializer = await deserializerFactory
            .CreateAsync(command.ContentTypeId, contentfulService, cancellationToken);

        await contentUploader.UploadAsync(
            CreateContentUploadRequest(command, input, deserializer, contentfulService),
            cancellationToken);
    }

    private static DataSource GetFileSource(ContentUploadCommand command)
    {
        var fileExtension = Path.GetExtension(command.InputPath) ?? string.Empty;
        return DataSource.From(fileExtension);
    }

    private static ContentUploadRequest CreateContentUploadRequest(
        ContentUploadCommand command,
        Content input,
        IContentEntryDeserializer deserializer,
        IContentfulService contentfulService)
    {
        return new ContentUploadRequest
        {
            Content = input,
            Publish = command.ShouldPublish,
            Deserializer = deserializer,
            ContentfulService = contentfulService
        };
    }
}
