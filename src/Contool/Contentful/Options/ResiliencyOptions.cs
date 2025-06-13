namespace Contool.Contentful.Options;

public class ResiliencyOptions
{
    public RetryPolicyOptions RetryPolicy { get; init; } = new();

    public ConcurrencyLimiterOptions ConcurrencyLimiter { get; init; } = new();

    public class RetryPolicyOptions
    {
        public int RetryCount { get; init; } = 3;

        public string BackoffStrategy { get; init; } = "Exponential";

        public double BaseDelaySeconds { get; init; } = 2;
    }

    public class ConcurrencyLimiterOptions
    {
        public int ConcurrencyLimit { get; init; } = 4;
    }
}
