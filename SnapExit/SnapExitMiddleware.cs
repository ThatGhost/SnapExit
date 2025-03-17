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
            var token = excecutionControlService.GetToken();

            try
            {
                // Setup task race
                using var tokenTask = Task.Delay(Timeout.Infinite, token);
                var task = _next(context);
                var completedTask = await Task.WhenAny(task, tokenTask);

                if (completedTask == task)
                {
                    excecutionControlService.StopExecution();
                    return;
                }

                // Early ckecks
                var responseData = excecutionControlService.GetResponseData();
                if (!token.IsCancellationRequested) return;
                if (context.Response.HasStarted) return;

                // write response 
                await WriteResponse(context, serializer, excecutionControlService);
            }
            catch (Exception)
            {
                throw;
            }
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
