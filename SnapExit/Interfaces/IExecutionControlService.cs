using System.Diagnostics.CodeAnalysis;

namespace SnapExit.Interfaces;

public interface IExecutionControlService
{
    public object EnvironmentData { get; set; }

    /// <summary>
    /// Stops the current request execution and returns the provided response to the client
    /// </summary>
    /// <param name="customResponseData">the response to send to the client</param>
    [DoesNotReturn]
    public Task StopExecution(object customResponseData);

    /// <summary>
    /// Stops the current request execution and returns the default response to the client
    /// </summary>
    [DoesNotReturn]
    public Task StopExecution();

    public CancellationTokenSource GetTokenSource();
}
