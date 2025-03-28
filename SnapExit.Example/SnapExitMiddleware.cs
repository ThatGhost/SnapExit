using SnapExit.Example.Entities;
using SnapExit.Services;
using System.Text.Json;

namespace SnapExit.Example;

public sealed class SnapExitMiddleware : SnapExitManager<CustomResponseData, HttpContext>
{
    private readonly RequestDelegate _next;

    public SnapExitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context, ExecutionControlService executionControlService)
    {
        // SnapExit specific setup for middleware
        executionControlService.EnvironmentData = context;

        // now SnapExit flings into action
        RegisterSnapExit(_next(context), executionControlService);
        return Task.CompletedTask;
    }

    protected override void SnapExitResponse(object? sender, OnSnapExitEventArgs args)
    {
        var response = args.ResponseData;
        var context = args.EnvironmentData;

        if (response is null)
            throw new Exception("Something went wrong with state");
        if (context is null)
            throw new Exception("Something went wrong with environment");

        if (context.Response.HasStarted) return;

        context.Response.StatusCode = response.StatusCode;

        if (response.Headers is not null)
        {
            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }
        }

        if (response.Body is not null)
        {
            args.AddTask(context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(response.Body)));
        }
    }
}
