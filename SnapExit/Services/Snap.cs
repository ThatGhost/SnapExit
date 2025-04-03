using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SnapExit.Services;

public static class Snap
{
    private static readonly ConcurrentQueue<(CancellationTokenSource,bool)> _cancellationTokenSource = new();
    private static ConcurrentDictionary<CancellationToken, object?> ResponseData { get; set; } = new();

    public static object? GetResponseData(CancellationToken token)
    {
        return ResponseData.TryGetValue(token, out object? value) ? value : null;
    }

    internal static (CancellationTokenSource, bool) GetTokenSource(bool isManager) {
        if(_cancellationTokenSource.TryDequeue(out (CancellationTokenSource, bool) cts))
            return cts;

        (CancellationTokenSource, bool) newCts = new(new(), isManager);
        _cancellationTokenSource.Enqueue(newCts);
        return newCts;
    }

    /// <summary>
    /// Stops the current request execution and returns the provided response to the client
    /// </summary>
    /// <param name="customResponseData">the response to send to the client</param>
#pragma warning disable CS8763 // A method marked [DoesNotReturn] should not return.
    [DoesNotReturn]
    public static async Task Exit(object customResponseData)
    {
        var cts = GetTokenSource(false);
        ResponseData.TryAdd(cts.Item1.Token, customResponseData);
        if(cts.Item2) cts.Item1.Cancel();
        await Task.Delay(Timeout.Infinite);
    }

    /// <summary>
    /// Stops the current request execution and returns the default response to the client
    /// </summary>
    [DoesNotReturn]
    public static async Task Exit()
    {
        var cts = GetTokenSource(false);
        if (cts.Item2) cts.Item1.Cancel();
        await Task.Delay(Timeout.Infinite);
    }
#pragma warning restore CS8763 // A method marked [DoesNotReturn] should not return.
}
