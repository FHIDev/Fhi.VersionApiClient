using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;


namespace Fhi.Common.VersionApiClient;

/// <summary>
/// Contains extension methods for configuring version-related services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds version client services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="systemName">The name of the system.</param>
    /// <param name="componentName">The name of the component.</param>
    /// <param name="versionapiurl">Url to the hosted (azure) version api</param>
    /// <returns>The <see cref="IServiceCollection"/> after the services have been added.</returns>
    public static VersionClientFactory AddVersionClient(this IServiceCollection services, string systemName, string componentName, string versionapiurl)
    {
        ClientVersionService.InitializeNames(systemName, componentName);
        services.AddScoped<IClientVersionService, ClientVersionService>();
        services.AddScoped<IVersionService, VersionService>();
        var factory = new VersionClientFactory(services, versionapiurl);
        return factory;
    }

    /// <summary>
    /// Adds version client services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="vc"><see cref="VersionApiConfiguration"/> </param>
    /// <returns>The <see cref="IServiceCollection"/> after the services have been added.</returns>
    public static VersionClientFactory AddVersionClient(this IServiceCollection services, VersionApiConfiguration vc) 
        => AddVersionClient(services, vc.SystemName, vc.ComponentName, vc.VersionApiUrl);

    /// <summary>
    /// Adds version client services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="systemName">The name of the system.</param>
    /// <param name="componentName">The name of the component.</param>
    /// <param name="config"><see cref="IConfiguration"/> with a VersionApiUrl string in appsettings</param>
    /// <returns>The <see cref="IServiceCollection"/> after the services have been added.</returns>
    public static VersionClientFactory AddVersionClient(this IServiceCollection services, string systemName, string componentName, IConfiguration config)
    {
        var baseUrl = config["VersionApiUrl"];
        return baseUrl == null
            ? throw new ArgumentException("VersionApiUrl not found in configuration")
            : AddVersionClient(services, systemName, componentName, baseUrl);
    }

    /// <summary>
    /// Adds version client services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="config"><see cref="IConfiguration"/> with a VersionApiConfiguration section in appsettings</param>
    /// <returns>The <see cref="IServiceCollection"/> after the services have been added.</returns>
    public static VersionClientFactory AddVersionClient(this IServiceCollection services, IConfiguration config)
    {
        var vcs = config.GetSection(nameof(VersionApiConfiguration));
        var vc = vcs.Get<VersionApiConfiguration>() ?? throw new ArgumentException("VersionApiConfiguration not found in configuration");
        var baseUrl = vc.VersionApiUrl;
        return baseUrl == null
            ? throw new ArgumentException("VersionApiUrl not found in configuration")
            : AddVersionClient(services, vc.SystemName, vc.ComponentName, baseUrl);
    }
}

/// <summary>
/// Factory for creating a version client
/// </summary>
/// <param name="services"></param>
/// <param name="baseUrl"></param>
public class VersionClientFactory(IServiceCollection services, string baseUrl)
{

    /// <summary>
    /// Adds a hosted version API service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    public VersionClientFactory WithHostedPush()
    {
        services.AddHostedService<HostedVersionService>();
        return this;
    }

    /// <summary>
    /// Adds http client services for the version API with no authentication to the specified <see cref="IServiceCollection"/>.
    /// PS. Using a Refit version API
    /// </summary>
    public VersionClientFactory WithHttpClientService()
    {
        services.AddRefitClient<IVersionApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
        return this;
    }


}