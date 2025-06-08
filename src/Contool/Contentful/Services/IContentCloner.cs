using Contool.Contentful.Models;

namespace Contool.Contentful.Services;

internal interface IContentCloner
{
    Task CloneContentEntriesAsync(ContentEntryCloneRequest request, CancellationToken cancellationToken);
}
