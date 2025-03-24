﻿using Microsoft.AspNetCore.Http;
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
        executionControlService.EnviroumentData = context;

        // get the task of the request
        var task = _next(context);

        // now SnapExit flings into action
        RegisterSnapExit(task, linkedCts, executionControlService);
        return Task.CompletedTask;
    }

    protected override async Task SnapExitResponse(object? responseData, object? enviroumentData)
    {
        if (responseData is not CustomResponseData response) 
            throw new Exception("Something went wrong with state");
        if (enviroumentData is not HttpContext context)
            throw new Exception("Something went wrong with enviroument");

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
