using SnapExit.Entities;
using SnapExit.Interfaces;
using SnapExit.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSnapExit<T>(this IServiceCollection services, Action<SnapExitOptions<T>>? options = null)
            where T : class
        {
            services.Configure<SnapExitOptions<T>>(configoptions =>
            {
                options?.Invoke(configoptions);
            });

            services.AddScoped<IExecutionControlService, ExecutionControlService>();
            services.AddScoped<ExecutionControlService>(provider => (ExecutionControlService)provider.GetRequiredService<IExecutionControlService>());
            return services;
        }
    }
}
