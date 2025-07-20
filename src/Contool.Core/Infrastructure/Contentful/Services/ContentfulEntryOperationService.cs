using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryOperationService(
    IContentfulManagementClientAdapter client) : IContentfulEntryOperationService
{
    public async Task<OperationResult> CreateOrUpdateEntryAsync(
        Entry<dynamic> entry,
        int version,
        CancellationToken cancellationToken = default)
    {
        var entryId = entry.GetId()!;

        try
        {
            if (entry.IsArchived())
            {
                var unarchived = await client.UnarchiveEntryAsync(
                    entryId: entryId,
                    version: version,
                    cancellationToken: cancellationToken);

                version = unarchived.GetVersion();
            }

            await client.CreateOrUpdateEntryAsync(
                entry,
                version: version,
                cancellationToken: cancellationToken);

            return OperationResult.Success(entryId, Operation.Upload);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(entryId, Operation.Upload, ex);
        }
    }

    public async Task<OperationResult> PublishEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default)
    {
        var entryId = entry.GetId()!;

        try
        {
            if (entry.IsPublished() || entry.IsArchived())
            {
                return OperationResult.Success(entryId, Operation.Publish);
            }

            await client.PublishEntryAsync(
                entryId: entryId,
                version: entry.GetVersion(),
                cancellationToken: cancellationToken);

            return OperationResult.Success(entryId, Operation.Publish);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(entryId, Operation.Publish, ex);
        }
    }

    public async Task<OperationResult> UnpublishEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default)
    {
        var entryId = entry.GetId()!;

        try
        {
            if (!entry.IsPublished())
            {
                return OperationResult.Success(entryId, Operation.Unpublish);
            }

            await client.UnpublishEntryAsync(
                entryId: entryId,
                version: entry.GetVersion(),
                cancellationToken: cancellationToken);

            return OperationResult.Success(entryId, Operation.Unpublish);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(entryId, Operation.Unpublish, ex);
        }
    }

    public async Task<OperationResult> DeleteEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default)
    {
        var entryId = entry.GetId()!;

        try
        {
            if (entry.IsArchived())
            {
                entry = await client.UnarchiveEntryAsync(
                    entryId: entryId,
                    version: entry.GetVersion(),
                    cancellationToken: cancellationToken);
            }

            await client.DeleteEntryAsync(
                entryId: entryId,
                version: entry.GetVersion(),
                cancellationToken: cancellationToken);

            return OperationResult.Success(entryId, Operation.Delete);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(entryId, Operation.Delete, ex);
        }
    }
}