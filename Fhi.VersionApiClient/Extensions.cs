using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Common.VersionApiClient;

/// <summary>
/// Extension methods for the version API client
/// Use these for setting up the version API client in your application
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Setup dependency injection for the version API client
    /// </summary>
    /// <param name="services"></param>
    public static void AddVersionApiClient(this IServiceCollection services)
    {
        services.AddScoped<IVersionService, VersionService>();
    }
}