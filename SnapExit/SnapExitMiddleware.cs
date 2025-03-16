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
        private readonly ExecutionControlService _excecutionControlService;

        public SnapExitMiddleware(RequestDelegate next, ExecutionControlService excecutionControlService)
        {
            _next = next;
            _excecutionControlService = excecutionControlService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = _excecutionControlService.GetToken();

            try
            {
                // Setup task race
                var task = _next(context);
                using var tokenTask = Task.Delay(Timeout.Infinite, token);
                var completedTask = await Task.WhenAny(task, tokenTask);

                // Normal request
                if (completedTask == task)
                {
                    await task;
                    return;
                }

                // Eearly ckecks
                var responseData = _excecutionControlService.GetResponseData();
                if (!token.IsCancellationRequested) return;
                if (context.Response.HasStarted) return;
                if (responseData == null) return;

                // write response 
                await WriteResponse(context, responseData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task WriteResponse(HttpContext context, CustomResponseData responseData)
        {
            // Write response
            context.Response.StatusCode = responseData.StatusCode;

            if (responseData.Headers != null)
            {
                foreach (var header in responseData.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value;
                }
            }

            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(responseData.Body);
            await context.Response.WriteAsync(json);
        }
    }
}
