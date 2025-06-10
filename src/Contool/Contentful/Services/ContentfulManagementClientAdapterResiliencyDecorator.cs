using Contentful.Core.Errors;
using Contentful.Core.Models;
using Polly;
using Polly.RateLimit;
using Polly.Wrap;

using Locale = Contentful.Core.Models.Management.Locale;

namespace Contool.Contentful.Services;

internal class ContentfulManagementClientAdapterResiliencyDecorator(
    IContentfulManagementClientAdapter innerAdapter) : IContentfulManagementClientAdapter
{
    private readonly AsyncPolicyWrap _resiliencePolicy = CreateResiliencePolicy();

    public async Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.GetLocalesCollectionAsync(ct), cancellationToken);

    public async Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.GetContentTypeAsync(contentTypeId, ct), cancellationToken);

    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.GetContentTypesAsync(ct), cancellationToken);

    public async Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.CreateOrUpdateContentTypeAsync(contentType, ct), cancellationToken);

    public async Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.ActivateContentTypeAsync(contentTypeId, version, ct), cancellationToken);

    public async Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.DeleteContentTypeAsync(contentTypeId, ct), cancellationToken);

    public async Task<IEnumerable<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.GetEntriesCollectionAsync(queryString, ct), cancellationToken);

    public async Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.CreateOrUpdateEntryAsync(entry, version, ct), cancellationToken);

    public async Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.PublishEntryAsync(entryId, version, ct), cancellationToken);

    public async Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => await _resiliencePolicy.ExecuteAsync(ct => innerAdapter.DeleteEntryAsync(entryId, version, ct), cancellationToken);

    private static AsyncPolicyWrap CreateResiliencePolicy()
    {
        var retryPolicy = Policy
            .Handle<ContentfulException>()
            .Or<RateLimitRejectedException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var rateLimiterPolicy = Policy.RateLimitAsync(10, TimeSpan.FromSeconds(1)); // Example: 10 requests per second

        return Policy.WrapAsync(retryPolicy, rateLimiterPolicy);
    }
}