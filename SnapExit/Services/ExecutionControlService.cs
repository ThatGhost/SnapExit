using SnapExit.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SnapExit.Services;

public sealed class ExecutionControlService : IExecutionControlService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    internal object? ResponseData { get; private set; }

    public CancellationTokenSource GetTokenSource() => _cancellationTokenSource;

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public object? EnviroumentData { get; set; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

    /// <summary>
    /// Stops the current request execution and returns the provided response to the client
    /// </summary>
    /// <param name="customResponseData">the response to send to the client</param>
    [DoesNotReturn]
    public void StopExecution(object customResponseData)
    {
        ResponseData = customResponseData;
        StopExecution();
    }

    /// <summary>
    /// Stops the current request execution and returns the default response to the client
    /// </summary>
    [DoesNotReturn]
    public async void StopExecution()
    {
        _cancellationTokenSource.Cancel();
        await Task.Delay(-1);
    }
}
