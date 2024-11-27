using Microsoft.Extensions.Logging;

namespace Fhi.VersionApiClient;

/// <summary>
/// Interface for DI for the ApiVersionService
/// </summary>
public interface IClientVersionService
{
    /// <summary>
    /// Uploads the version information.
    /// Note that the System and Component names must be set before calling this method.
    /// Extracts the current environment from the ASPNETCORE_ENVIRONMENT environment variable.
    /// Extracts the version from the assembly file version.
    /// </summary>
    /// <param name="status">The status of the component (aka Health). See docs for a list of applicable values</param>
    Task<string> UploadVersionInfo(string status = "healthy");
}

/// <summary>
/// Service for managing API version information.
/// </summary>
public class ClientVersionService(IVersionService versionService, ILogger<ClientVersionService> logger)
    : IClientVersionService
{
    static string System { get; set; } = "";
    static string Component { get; set; } = "";

    /// <summary>
    /// Set the System and Component names for the application
    /// </summary>
    /// <param name="system"></param>
    /// <param name="component"></param>
    public static void InitializeNames(string system, string component)
    {
        System = system;
        Component = component;
    }

    /// <inheritdoc />
    public async Task<string> UploadVersionInfo(string status = "healthy")
    {
        if (string.IsNullOrEmpty(System) || string.IsNullOrEmpty(Component))
        {
            logger.LogError("{ApiVersionService} System or Component not set", nameof(ClientVersionService));
            return "System or Component not set";
        }

        logger.LogDebug(
            "{ApiVersionService} Uploading version information for {System}/{Component} with health {status}",
            nameof(ClientVersionService), System, Component, status);
        var result = await versionService.UploadVersionInformation(System, Component, status);
        return result;
    }
}