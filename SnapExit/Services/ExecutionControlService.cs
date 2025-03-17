using Microsoft.Extensions.Options;
using SnapExit.Entities;
using SnapExit.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace SnapExit.Services
{
    public class ExecutionControlService : IExecutionControlService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private CancellationToken Token => _cancellationTokenSource.Token;
        private CustomResponseData? ResponseData = null;

        internal CancellationToken GetToken() => Token;
        internal CustomResponseData GetResponseData() => ResponseData ?? DefaultReponse;

        public Func<object,string>? CustomResponseSerializer { get; set; }
        public CustomResponseData DefaultReponse { get; }

        public ExecutionControlService(IOptions<SnapExitOptions> options)
        {
            DefaultReponse = new CustomResponseData
            {
                StatusCode = options.Value.DefaultStatusCode,
                Body = options.Value.DefaultBody,
                Headers = options.Value.DefaultHeaders
            };
            CustomResponseSerializer = options.Value.CustomResponseSerializer;
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
        }
    }
}
