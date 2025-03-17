using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SnapExit.Entities;
using SnapExit.Interfaces;
using SnapExit.Services;
using SnapExit.Services.Serializers;

namespace SnapExit
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSnapExit<T>(this IServiceCollection services, Action<SnapExitOptions>? options = null) 
            where T : class, IResponseBodySerializer
        {
            services.Configure<SnapExitOptions>(configoptions =>
            {
                options?.Invoke(configoptions);
            });

            services.AddTransient<IResponseBodySerializer, T>();
            services.AddScoped<IExecutionControlService, ExecutionControlService>();
            services.AddScoped<ExecutionControlService>(provider => (ExecutionControlService)provider.GetRequiredService<IExecutionControlService>());
            return services;
        }

        public static IServiceCollection AddSnapExit(this IServiceCollection services, Action<SnapExitOptions>? options = null)
        {
            return services.AddSnapExit<JSONResponseBodySerializer>(options);
        }

        public static void AddSnapExit(this IApplicationBuilder app)
        {
            app.UseMiddleware<SnapExitMiddleware>();
        }
    }
}
