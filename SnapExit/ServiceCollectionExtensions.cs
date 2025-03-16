using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SnapExit.Entities;
using SnapExit.Interfaces;
using SnapExit.Services;

namespace SnapExit
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSnapExit(this IServiceCollection services, Action<SnapExitOptions>? options = null)
        {
            services.Configure<SnapExitOptions>(configoptions =>
            {
                options?.Invoke(configoptions);
            });

            services.AddScoped<IExecutionControlService, ExecutionControlService>();
            services.AddScoped<ExecutionControlService>(provider => (ExecutionControlService)provider.GetRequiredService<IExecutionControlService>());
            return services;
        }

        public static void AddSnapExit(this IApplicationBuilder app)
        {
            app.UseMiddleware<SnapExitMiddleware>();
        }
    }
}
