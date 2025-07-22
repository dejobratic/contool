using Contentful.Core.Errors;
using Contool.Core.Infrastructure.Contentful.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Contool.Core.Infrastructure.Utils.Services;

public class ResiliencyExecutor(IOptions<ResiliencyOptions> options) : IResiliencyExecutor
{
    private readonly AsyncRetryPolicy _retryPolicy = CreateRetryPolicy(options.Value.RetryPolicy);
    private readonly SemaphoreSlim _concurrencyLimiter = new(options.Value.ConcurrencyLimiter.ConcurrencyLimit);

    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await _concurrencyLimiter.WaitAsync(cancellationToken);
        try
        {
            return await _retryPolicy.ExecuteAsync(action, cancellationToken);
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        await _concurrencyLimiter.WaitAsync(cancellationToken);
        try
        {
            await _retryPolicy.ExecuteAsync(action, cancellationToken);
        }
        finally
        {
            _concurrencyLimiter.Release();
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