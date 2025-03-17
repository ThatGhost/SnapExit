using Microsoft.AspNetCore.TestHost;
using System.Diagnostics;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace SnapExit.Benchmark
{
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
                    app.UseSnapExit();
                    app.UseRouting();
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
        public async Task Middleware_Should_Snap_Exit()
        {
            // Arrange
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await _client.GetAsync("BenchMark/SnapExit");

            stopwatch.Stop();

            // Log the timings and counts
            _logger.LogInformation($"Total Execution Time for Exception Test: {stopwatch.ElapsedMilliseconds} ms");
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Middleware_Should_Throw_Exception()
        {
            // Arrange
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await _client.GetAsync("BenchMark/Exception");

            stopwatch.Stop();

            // Log the timings and counts
            _logger.LogInformation($"Total Execution Time for Exception Test: {stopwatch.ElapsedMilliseconds} ms");
        }

        [Fact]
        public async Task Middleware_Should_Result()
        {
            // Arrange
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.LogInformation("HERE");

            var result = await _client.GetAsync("BenchMark/Result");

            stopwatch.Stop();

            // Log the timings and counts
            _logger.LogInformation($"Total Execution Time for Exception Test: {stopwatch.ElapsedMilliseconds} ms");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        }
    }

}
