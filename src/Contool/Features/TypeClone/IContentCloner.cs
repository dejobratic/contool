namespace Contool.Features.TypeClone;

internal interface IContentCloner
{
    Task CloneContentEntriesAsync(ContentEntryCloneRequest request, CancellationToken cancellationToken);
}
