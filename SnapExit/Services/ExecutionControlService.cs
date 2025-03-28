using SnapExit.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SnapExit.Services;

public sealed class ExecutionControlService : IExecutionControlService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    internal object? ResponseData { get; private set; }

    public CancellationTokenSource GetTokenSource() => _cancellationTokenSource;

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public object? EnvironmentData { get; set; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

    /// <summary>
    /// Stops the current request execution and returns the provided response to the client
    /// </summary>
    /// <param name="customResponseData">the response to send to the client</param>
    [DoesNotReturn]
    public Task StopExecution(object customResponseData)
    {
        ResponseData = customResponseData;
        return StopExecution();
    }

    /// <summary>
    /// Stops the current request execution and returns the default response to the client
    /// </summary>
    [DoesNotReturn]
    public async Task StopExecution()
#pragma warning disable CS8763 // A method marked [DoesNotReturn] should not return.
    {
        _cancellationTokenSource.Cancel();
        await Task.Delay(Timeout.Infinite);
    }
#pragma warning restore CS8763 // A method marked [DoesNotReturn] should not return.
}
