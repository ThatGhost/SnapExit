using Microsoft.AspNetCore.TestHost;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using SnapExit.Tests.Services;
using SnapExit.Tests.Entities;

namespace SnapExit.Benchmark;

[Collection("Sequential")]
#pragma warning disable CS0657 // Not a valid attribute location for this declaration
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#pragma warning restore CS0657 // Not a valid attribute location for this declaration
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
                //services.AddSnapExit();
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

    [Fact(Skip = "for some reason the call back gets stolen by a diff test")]
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

    [Fact(Skip = "w")]
    public async Task Service_ShouldSnapExit()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();
        TestResponseObject reponse = new TestResponseObject() { Message = "This is a message that has passed" };

        // Act
        await testService.SnapExit_SingleResponseTest(reponse);

        // Assert
        Assert.Equal(reponse, testService.response);
    }

    [Fact(Skip = "w")]
    public async Task Service_ShouldSnapExit_TimeDifferentSnapExits()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();

        // Act
        var t1 = testService.SnapExit_MultipleResponseTest(500, new TestResponseObject() { Message = "1" });
        var t2 = testService.SnapExit_MultipleResponseTest(0, new TestResponseObject() { Message = "2" });
        var t3 = testService.SnapExit_MultipleResponseTest(100, new TestResponseObject() { Message = "3" });

        // Assert
        await Task.WhenAll(t1, t2, t3);
    }

    [Fact(Skip = "w")]
    public async Task Service_ShouldSnapExit_ConcurrentSnapExits()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();

        // Act
        var t1 = testService.SnapExit_MultipleResponseTest(500, new TestResponseObject() { Message = "1" });
        var t2 = testService.SnapExit_MultipleResponseTest(500, new TestResponseObject() { Message = "2" });
        var t3 = testService.SnapExit_MultipleResponseTest(500, new TestResponseObject() { Message = "3" });

        // Assert
        await Task.WhenAll(t1, t2, t3);
    }

    [Fact(Skip = "i dont have a solve for this yet")]
    public async Task Service_ShouldSnapExit_ImmediateSnapExits()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();

        // Act
        var t1 = testService.SnapExit_MultipleResponseTest(0, new TestResponseObject() { Message = "1" });
        var t2 = testService.SnapExit_MultipleResponseTest(0, new TestResponseObject() { Message = "2" });
        var t3 = testService.SnapExit_MultipleResponseTest(0, new TestResponseObject() { Message = "3" });

        // Assert
        await Task.WhenAll(t1, t2, t3);
    }
}
