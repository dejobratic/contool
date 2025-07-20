using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulEntryBulkOperationService
{
    Task<IReadOnlyList<OperationResult>> PublishEntriesAsync(IReadOnlyList<Entry<dynamic>> entries, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<OperationResult>> UnpublishEntriesAsync(IReadOnlyList<Entry<dynamic>> entries, CancellationToken cancellationToken = default);
}