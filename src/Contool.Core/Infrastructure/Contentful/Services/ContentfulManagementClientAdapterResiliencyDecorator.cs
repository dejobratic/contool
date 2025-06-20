﻿using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

using Locale = Contentful.Core.Models.Management.Locale;
using Policy = Polly.Policy;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterResiliencyDecorator(
    IOptions<ResiliencyOptions> resiliencyOptions,
    IContentfulManagementClientAdapter innerAdapter) : IContentfulManagementClientAdapter
{
    private readonly AsyncRetryPolicy _retryPolicy = CreateRetryPolicy(resiliencyOptions.Value.RetryPolicy);
    private readonly SemaphoreSlim _concurrencySemaphore = new(resiliencyOptions.Value.ConcurrencyLimiter.ConcurrencyLimit);

    public async Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.GetSpaceAsync(spaceId, ct), cancellationToken);

    public async Task<ContentfulEnvironment> GetEnvironmentAsync(string environmentId, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.GetEnvironmentAsync(environmentId, ct), cancellationToken);

    public async Task<User> GetCurrentUser(CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.GetCurrentUser(ct), cancellationToken);

    public async Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.GetLocalesCollectionAsync(ct), cancellationToken);

    public async Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.GetContentTypeAsync(contentTypeId, ct), cancellationToken);

    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.GetContentTypesAsync(ct), cancellationToken);

    public async Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.CreateOrUpdateContentTypeAsync(contentType, ct), cancellationToken);

    public async Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.ActivateContentTypeAsync(contentTypeId, version, ct), cancellationToken);

    public async Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.DeactivateContentTypeAsync(contentTypeId, ct), cancellationToken);

    public async Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.DeleteContentTypeAsync(contentTypeId, ct), cancellationToken);

    public async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.GetEntriesCollectionAsync(queryString, ct), cancellationToken);

    public async Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.CreateOrUpdateEntryAsync(entry, version, ct), cancellationToken);

    public async Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.PublishEntryAsync(entryId, version, ct), cancellationToken);

    public async Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.UnpublishEntryAsync(entryId, version, ct), cancellationToken);

    public async Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.ArchiveEntryAsync(entryId, version, ct), cancellationToken);

    public async Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.UnarchiveEntryAsync(entryId, version, ct), cancellationToken);

    public async Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => await ExecuteAsync(ct => innerAdapter.DeleteEntryAsync(entryId, version, ct), cancellationToken);

    private async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        await _concurrencySemaphore.WaitAsync(cancellationToken);
        try
        {
            return await _retryPolicy.ExecuteAsync(action, cancellationToken);
        }
        finally
        {
            _concurrencySemaphore.Release();
        }
    }

    private async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        await _concurrencySemaphore.WaitAsync(cancellationToken);
        try
        {
            await _retryPolicy.ExecuteAsync(action, cancellationToken);
        }
        finally
        {
            _concurrencySemaphore.Release();
        }
    }

    private static AsyncRetryPolicy CreateRetryPolicy(ResiliencyOptions.RetryPolicyOptions options)
    {
        return options.BackoffStrategy switch
        {
            "Exponential" => Policy
                .Handle<ContentfulException>()
                .Or<ContentfulRateLimitException>()
                .WaitAndRetryAsync(options.RetryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * options.BaseDelaySeconds)),

            "Fixed" => Policy
                .Handle<ContentfulException>()
                .Or<ContentfulRateLimitException>()
                .WaitAndRetryAsync(options.RetryCount, _ =>
                    TimeSpan.FromSeconds(options.BaseDelaySeconds)),

            _ => Policy
                .Handle<ContentfulException>()
                .Or<ContentfulRateLimitException>()
                .RetryAsync(options.RetryCount)
        };
    }
}
