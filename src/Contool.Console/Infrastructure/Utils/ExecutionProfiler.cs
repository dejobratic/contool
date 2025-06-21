using System.Diagnostics;

namespace Contool.Console.Infrastructure.Utils;

public static class ExecutionProfiler
{
    public static async Task<MeasuredResult<T>> ProfileAsync<T>(
        Func<Task<T>> action,
        bool forceFullCollection = true)
    {
        // Start measuring time and memory
        var stopwatch = Stopwatch.StartNew();
        var initialMemory = GC.GetTotalMemory(forceFullCollection);

        var result = await action();

        // Stop measuring time and memory
        stopwatch.Stop();
        var finalMemory = GC.GetTotalMemory(forceFullCollection);

        return new MeasuredResult<T>(
            result,
            stopwatch.Elapsed,
            finalMemory - initialMemory);
    }
}

public class MeasuredResult<T>(T result, TimeSpan elapsedTime, long memoryUsageInBytes)
{
    public T Result { get; } = result;

    public TimeSpan ElapsedTime { get; } = elapsedTime;

    public string FormattedElapsedTime { get; } = FormatTime(elapsedTime);

    public long MemoryUsageInBytes { get; } = memoryUsageInBytes;

    public string FormatedMemoryUsage { get; set; } = FormatMemory(memoryUsageInBytes);

    private static string FormatTime(TimeSpan elapsed)
        => $"{elapsed.Hours}h {elapsed.Minutes}m {elapsed.Seconds}s";

    private static string FormatMemory(long bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        return bytes switch
        {
            >= GB => $"{bytes / (double)GB:F2} GB",
            >= MB => $"{bytes / (double)MB:F2} MB",
            >= KB => $"{bytes / (double)KB:F2} KB",
            _ => $"{bytes} Bytes"
        };
    }
}
