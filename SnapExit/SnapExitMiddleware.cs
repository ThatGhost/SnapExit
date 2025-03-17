using Microsoft.AspNetCore.Http;
using SnapExit.Entities;
using SnapExit.Interfaces;
using SnapExit.Services;
using System.Text.Json;

namespace SnapExit
{
    internal class SnapExitMiddleware
    {
        private readonly RequestDelegate _next;

        public SnapExitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ExecutionControlService excecutionControlService, IResponseBodySerializer serializer)
        {
            var cts = excecutionControlService.GetTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted, cts.Token);

            // Setup task race
            using var tokenTask = Task.Delay(Timeout.Infinite, linkedCts.Token);
            var task = _next(context);
            try
            {
                var completedTask = await Task.WhenAny(task, tokenTask);
                if (completedTask == task)
                {
                    excecutionControlService.StopExecution();
                    return;
                }
            }
            catch (Exception)
            {
                if (!tokenTask.IsCanceled) throw;
            }

            // Early ckecks
            var responseData = excecutionControlService.GetResponseData();
            if (!linkedCts.Token.IsCancellationRequested) return;
            if (context.Response.HasStarted) return;

            // write response 
            await WriteResponse(context, serializer, excecutionControlService);
        }

        private async Task WriteResponse(HttpContext context, IResponseBodySerializer serializer, ExecutionControlService excecutionControlService)
        {
            // Write response
            var responseData = excecutionControlService.GetResponseData();
            if (responseData == null) return;
            context.Response.StatusCode = responseData.StatusCode;

            if (responseData.Headers != null)
            {
                foreach (var header in responseData.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value;
                }
            }

            context.Response.ContentType = serializer.ContentType;
            await context.Response.WriteAsync(serializer.GetBody(responseData));
        }
    }
}
