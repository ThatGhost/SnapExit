using Microsoft.AspNetCore.Http;
using SnapExit.Example.Entities;
using SnapExit.Services;
using System.Text.Json;

namespace SnapExit.Benchmark;

public sealed class SnapExitMiddleware : SnapExitManager
{
    private readonly RequestDelegate _next;

    public SnapExitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context, ExecutionControlService executionControlService)
    {
        // Create a linked token for ASP.NET Core API to work
        var cts = executionControlService.GetTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted, cts.Token);

        // SnapExit specific setup
        executionControlService.EnviromentData = context;

        // get the task of the request
        var task = _next(context);

        // now SnapExit flings into action
        RegisterSnapExit(task, linkedCts, executionControlService);
        return Task.CompletedTask;
    }

    protected override async Task SnapExitResponse(object? stateData, object? enviroumentData)
    {
        var response = stateData as CustomResponseData;
        if (response is null) throw new Exception("Something went wrong with state");
        var context = enviroumentData as HttpContext;
        if (context is null) throw new Exception("Something went wrong with enviroument");

        if (context.Response.HasStarted) return;

        context.Response.StatusCode = response.StatusCode;

        if (response.Headers is not null)
        {
            foreach (var header in response.Headers) {
                context.Response.Headers[header.Key] = header.Value;
            }
        }

        if (response.Body is not null)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(response.Body));
        }
    }
}
