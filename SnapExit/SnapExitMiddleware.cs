using Microsoft.AspNetCore.Http;
using SnapExit.Entities;
using SnapExit.Interfaces;
using SnapExit.Services;

namespace SnapExit
{
    internal sealed class SnapExitMiddleware
    {
        private readonly RequestDelegate _next;

        public SnapExitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ExecutionControlService executionControlService, IResponseBodySerializer serializer)
        {
            var cts = executionControlService.GetTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted, cts.Token);

            // Setup task race
            using var tokenTask = Task.Delay(Timeout.Infinite, linkedCts.Token);
            var task = _next(context);
            try
            {
                var completedTask = await Task.WhenAny(task, tokenTask);
                if (completedTask == task)
                {
                    executionControlService.StopExecution();
                    return;
                }
            }
            catch (SnapExitException)
            {
                if (!tokenTask.IsCanceled) throw;
            }

            // Early ckecks
            if (!linkedCts.Token.IsCancellationRequested) return;
            if (context.Response.HasStarted) return;

            // write response 
            await WriteResponse(context, serializer, executionControlService);
        }

        private async Task WriteResponse(HttpContext context, IResponseBodySerializer serializer, ExecutionControlService executionControlService)
        {
            // Write response
            var responseData = executionControlService.ResponseData;
            if (responseData == null) responseData = executionControlService.DefaultReponse;

            context.Response.StatusCode = responseData.StatusCode;

            if (responseData.Headers is not null)
            {
                foreach (var header in responseData.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value;
                }
            }

            if(responseData.Body is not null)
            {
                context.Response.ContentType = serializer.ContentType;
                await context.Response.WriteAsync(serializer.GetBody(responseData));
            }
        }
    }
}
