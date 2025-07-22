using System.Net.Http.Headers;
using System.Text;
using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Extensions;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulBulkClient(
    HttpClient client,
    ContentfulOptions options) : IContentfulBulkClient
{
    private const string ApiBaseUrl = "https://api.contentful.com";
    private static readonly TimeSpan PollDelay = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5);

    public Task<BulkActionResponse> PublishAsync(
        IReadOnlyList<BulkActionItemBase> items,
        CancellationToken cancellationToken = default) =>
        ExecuteBulkActionAsync(BulkActionType.Publish, items, cancellationToken);

    public Task<BulkActionResponse> UnpublishAsync(
        IReadOnlyList<BulkActionItemBase> items,
        CancellationToken cancellationToken = default) =>
        ExecuteBulkActionAsync(BulkActionType.Unpublish, items, cancellationToken);

    private async Task<BulkActionResponse> ExecuteBulkActionAsync(
        BulkActionType action,
        IReadOnlyList<BulkActionItemBase> items,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            entities = new
            {
                items = items.Select(i => new { Sys = i }),
            }
        };
        
        var bulkResponse = await SendRequestAsync(
            HttpMethod.Post,
            BuildActionUrl(action),
            payload.SerializeToJsonString(),
            cancellationToken);

        return await PollUntilCompleteAsync(bulkResponse.Sys.Id, cancellationToken);
    }

    private string BuildActionUrl(BulkActionType action) =>
        $"{ApiBaseUrl}/spaces/{options.SpaceId}/environments/{options.Environment}/bulk_actions/{action.ToString().ToLower()}";

    private string BuildStatusUrl(string actionId) =>
        $"{ApiBaseUrl}/spaces/{options.SpaceId}/environments/{options.Environment}/bulk_actions/actions/{actionId}";

    private async Task<BulkActionResponse> SendRequestAsync(
        HttpMethod method,
        string url,
        string? jsonBody,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ManagementApiKey);

        if (jsonBody is not null)
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content.DeserializeFromJsonString<BulkActionResponse>()
               ?? throw new InvalidOperationException("Failed to deserialize BulkActionResponse.");
    }

    private async Task<BulkActionResponse> PollUntilCompleteAsync(
        string bulkActionId,
        CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow + Timeout;

        while (DateTime.UtcNow < deadline)
        {
            var response = await SendRequestAsync(
                HttpMethod.Get,
                BuildStatusUrl(bulkActionId),
                null,
                cancellationToken);

            if (BulkActionStatus.TerminalStatusCodes.Contains(response.Sys.Status))
                return response;

            await Task.Delay(PollDelay, cancellationToken);
        }

        throw new TimeoutException($"Bulk action {bulkActionId} did not complete in {Timeout}.");
    }
}