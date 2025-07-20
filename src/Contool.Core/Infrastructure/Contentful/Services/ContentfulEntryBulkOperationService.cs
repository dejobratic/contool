using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Utils.Models;
using System.Net.Http.Headers;
using System.Text;
using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Extensions;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationService(
    HttpClient client,
    ContentfulOptions options) : IContentfulEntryBulkOperationService
{
    public async Task<IReadOnlyList<OperationResult>> PublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        if (entries.Count == 0)
            return [];

        try
        {
            var bulkResponse = await GetBulkActionResponseAsync(
                BulkActionType.Publish, entries, cancellationToken);

            var completedResponse = await WaitForCompletionAsync(
                bulkResponse.Id, TimeSpan.FromMinutes(5), cancellationToken);

            return ProcessBulkActionResponse(completedResponse, Operation.Publish);
        }
        catch (Exception ex)
        {
            return entries
                .Select(entry => OperationResult.Failure(entry.GetId()!, Operation.Publish, ex))
                .ToList();
        }
    }

    public async Task<IReadOnlyList<OperationResult>> UnpublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        if (entries.Count == 0)
            return [];

        try
        {
            var bulkResponse = await GetBulkActionResponseAsync(
                BulkActionType.Unpublish, entries, cancellationToken);

            var completedResponse = await WaitForCompletionAsync(
                bulkResponse.Id, TimeSpan.FromMinutes(5), cancellationToken);

            return ProcessBulkActionResponse(completedResponse, Operation.Unpublish);
        }
        catch (Exception ex)
        {
            return entries
                .Select(entry => OperationResult.Failure(entry.GetId()!, Operation.Unpublish, ex))
                .ToList();
        }
    }

    private async Task<BulkActionResponse> GetBulkActionResponseAsync(
        BulkActionType action,
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken)
    {
        var request = CreateBulkActionRequest(action, entries);
        return await CreateBulkActionAsync(request, cancellationToken);
    }

    private static BulkActionRequest CreateBulkActionRequest(
        BulkActionType action,
        IReadOnlyList<Entry<dynamic>> entries)
    {
        return new BulkActionRequest
        {
            Action = action,
            Entities = new
            {
                Items = entries.Select(entry => new
                {
                    Sys = new BulkActionItem
                    {
                        Id = entry.GetId()!,
                        Version = entry.GetVersion(),
                    },
                }),
            },
        };
    }

    private async Task<BulkActionResponse> CreateBulkActionAsync(
        BulkActionRequest request,
        CancellationToken cancellationToken = default)
    {
        var endpoint =
            $"https://api.contentful.com/spaces/{options.SpaceId}/environments/{options.Environment}/bulk_actions/{request.Action.ToString().ToLower()}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(request.Entities.SerializeToJsonString(), Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ManagementApiKey);

        using var response = await client.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content.DeserializeFromJsonString<BulkActionResponse>()
               ?? throw new InvalidOperationException("Failed to deserialize BulkActionResponse.");
    }

    private async Task<BulkActionResponse> GetBulkActionAsync(
        string bulkActionId,
        CancellationToken cancellationToken = default)
    {
        var endpoint =
            $"https://api.contentful.com/spaces/{options.SpaceId}/environments/{options.Environment}/bulk_actions/actions/{bulkActionId}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ManagementApiKey);

        using var response = await client.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content.DeserializeFromJsonString<BulkActionResponse>()
               ?? throw new InvalidOperationException("Failed to deserialize BulkActionResponse.");
    }

    private async Task<BulkActionResponse> WaitForCompletionAsync(
        string bulkActionId,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        var endTime = DateTime.UtcNow + timeout;
        var delay = TimeSpan.FromMilliseconds(100);

        while (DateTime.UtcNow < endTime)
        {
            var response = await GetBulkActionAsync(bulkActionId, cancellationToken);

            if (response.Status is BulkActionStatus.Succeeded or BulkActionStatus.Failed)
            {
                return response;
            }

            await Task.Delay(delay, cancellationToken);
        }

        throw new TimeoutException(
            $"Bulk action {bulkActionId} did not complete within the specified timeout of {timeout}");
    }

    private static List<OperationResult> ProcessBulkActionResponse(
        BulkActionResponse response,
        Operation operation)
    {
        if (response.Status == BulkActionStatus.Succeeded)
        {
            return response.Items
                .Select(item => OperationResult.Success(item.Id, operation))
                .ToList();
        }

        var error = new InvalidOperationException(response.Error ?? "Bulk action failed");
        
        return response.Items
            .Select(item => OperationResult.Failure(item.Id, operation, error))
            .ToList();
    }
}