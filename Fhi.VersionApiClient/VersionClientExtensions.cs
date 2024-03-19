using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Common.VersionApiClient;

/// <summary>
/// Contains extension methods for configuring version-related services.
/// </summary>
public static class VersionClientExtensions
{
    /// <summary>
    /// Adds version client services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="systemName">The name of the system.</param>
    /// <param name="componentName">The name of the component.</param>
    /// <returns>The <see cref="IServiceCollection"/> after the services have been added.</returns>
    public static IServiceCollection AddVersionClient(this IServiceCollection services, string systemName, string componentName)
    {
        ClientVersionService.InitializeNames(systemName,componentName);
        services.AddScoped<IClientVersionService, ClientVersionService>();
        services.AddScoped<IVersionService, VersionService>();
        return services;
    }
    
    /// <summary>
    /// Adds a hosted version API service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> after the service has been added.</returns>
    public static IServiceCollection AddPushVersionClient(this IServiceCollection services)
    {
        services.AddHostedService<HostedVersionService>();
        return services;
    }
}