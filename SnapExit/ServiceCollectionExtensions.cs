using SnapExit.Entities;
using SnapExit.Interfaces;
using SnapExit.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSnapExit(this IServiceCollection services)
    {
        services.AddScoped<IExecutionControlService, ExecutionControlService>();
        services.AddScoped<ExecutionControlService>(provider => (ExecutionControlService)provider.GetRequiredService<IExecutionControlService>());
        return services;
    }
}
