using SnapExit.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SnapExit.Interfaces;

public interface IExecutionControlService
{
    /// <summary>
    /// Stops the current request execution and returns the provided response to the client
    /// </summary>
    /// <param name="customResponseData">the response to send to the client</param>
    [DoesNotReturn]
    public void StopExecution(object customResponseData);

    /// <summary>
    /// Stops the current request execution and returns the default response to the client
    /// </summary>
    [DoesNotReturn]
    public void StopExecution();

    public CancellationTokenSource GetTokenSource();
}
