using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulEntryOperationService
{
    Task<OperationResult> CreateOrUpdateEntryAsync(
        Entry<dynamic> entry,
        int version,
        CancellationToken cancellationToken = default);

    Task<OperationResult> PublishEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default);

    Task<OperationResult> UnpublishEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default);

    Task<OperationResult> DeleteEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default);
}