using Microsoft.Extensions.Options;
using SnapExit.Entities;
using SnapExit.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SnapExit.Services
{
    public sealed class ExecutionControlService : IExecutionControlService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        internal CustomResponseData? ResponseData { get; private set; }

        internal CancellationTokenSource GetTokenSource() => _cancellationTokenSource;

        public CustomResponseData DefaultReponse { get; }

        public ExecutionControlService(IOptions<SnapExitOptions> options)
        {
            DefaultReponse = new CustomResponseData
            {
                StatusCode = options.Value.DefaultStatusCode,
                Body = options.Value.DefaultBody,
                Headers = options.Value.DefaultHeaders
            };
        }

        /// <summary>
        /// Stops the current request execution and returns the provided response to the client
        /// </summary>
        /// <param name="customResponseData">the response to send to the client</param>
        [DoesNotReturn]
        public void StopExecution(CustomResponseData customResponseData)
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
