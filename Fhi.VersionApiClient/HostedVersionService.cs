using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fhi.Common.VersionApiClient;

/// <summary>
/// Hosted service that calls the version API at startup
/// </summary>
/// <param name="serviceProvider"></param>
public class HostedVersionService(IServiceProvider serviceProvider) : IHostedService
{
    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        // Resolve the required service
        var apiService = scope.ServiceProvider.GetRequiredService<IClientVersionService>();
        string result = "";
        // Call the API method
        Task.Run(async () => result = await apiService.UploadVersionInfo(), cancellationToken);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}