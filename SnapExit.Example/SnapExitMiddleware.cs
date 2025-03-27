using Microsoft.AspNetCore.Http;
using SnapExit.Example.Entities;
using SnapExit.Services;
using System.Text.Json;

namespace SnapExit.Tests.Services;

public sealed class SnapExitMiddleware : SnapExitManager<CustomResponseData>
{
    private readonly RequestDelegate _next;

    public SnapExitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        OnSnapExit callback = async (response) =>
        {
            await WriteResponse(response, context);
        };

        onSnapExit += callback;
        await RegisterSnapExitAsync(_next(context));
        onSnapExit -= callback;
    }

    private async Task WriteResponse(CustomResponseData? response, HttpContext? context)
    {
        if (response is null)
            throw new Exception("Something went wrong with state");
        if (context is null)
            throw new Exception("Something went wrong with enviroument");

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
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(response.Body));
        }
    }
}
