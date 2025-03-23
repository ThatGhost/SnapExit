using Microsoft.Extensions.Options;
using SnapExit.Entities;
using SnapExit.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SnapExit.Services
{
    public sealed class ExecutionControlService : IExecutionControlService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        internal object? ResponseData { get; private set; }

        public CancellationTokenSource GetTokenSource() => _cancellationTokenSource;

        public object EnviroumentData { get; set; }

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
        public void StopExecution()
        {
            _cancellationTokenSource.Cancel();
            throw new SnapExitException();
        }
    }
}
