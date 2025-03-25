using Microsoft.AspNetCore.TestHost;
using System.Diagnostics;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using SnapExit.Example;
using SnapExit.Tests.Services;
using SnapExit.Tests.Entities;
using SnapExit.Services;

namespace SnapExit.Benchmark;

public class MiddlewareIntegrationTests
{
    private readonly TestServer _server;
    private readonly HttpClient _client;
    private readonly ILogger<MiddlewareIntegrationTests> _logger;

    public MiddlewareIntegrationTests()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(configure => configure.AddConsole());
                services.AddControllers();
                services.AddSnapExit();
            })
            .Configure(app =>
            {
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseMiddleware<SnapExitMiddleware>();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

        _server = new TestServer(builder);
        _client = _server.CreateClient();

        var serviceProvider = _server.Services;
        _logger = serviceProvider.GetRequiredService<ILogger<MiddlewareIntegrationTests>>();
    }

    [Fact]
    public async Task Middleware_Should_SnapExit()
    {
        // Arrange & Act
        var response = await _client.GetAsync("Test/SnapExit");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Middleware_Should_Not_SnapExit()
    {
        // Arrange & Act
        var response = await _client.GetAsync("Test/SnapExit/succes");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Service_ShouldSnapExit()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest(new ExecutionControlService());
        SnapExitReponse reponse = new SnapExitReponse() { Message = "This is a message that has passed" };

        // Act
        await testService.SetupSnapExit(reponse);

        // Assert
        Assert.Equal(reponse, testService.response);
        Assert.NotNull(testService.enviroument);
    }
}
