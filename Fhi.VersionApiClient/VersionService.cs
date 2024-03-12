using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Refit;

namespace Fhi.Common.VersionApiClient;

/// <summary>
/// Refit API definition for the azure version API
/// </summary>
public interface IVersionApi
{
    /// <summary>
    /// Uploads version information to the azure version API
    /// </summary>
    /// <param name="environment"></param>
    /// <param name="system"></param>
    /// <param name="component"></param>
    /// <param name="version"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    [Get("/api/SetInformation")]
    public Task SetInformation([Query] string environment, [Query] string system, [Query] string component, [Query] string version, [Query] string status);

    /// <summary>
    /// Get a Shields.io badge for the version of a component
    /// </summary>
    /// <param name="environment"></param>
    /// <param name="system"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    [Get("/api/GetInformation")]
    public Task<ApiResponse<ShieldsIo>> GetInformation([Query] string environment, [Query] string system, [Query] string component);
}

/// <summary>
/// Interface for the version service
/// </summary>
public interface IVersionService
{
    /// <summary>
    /// Uploads version information to the azure version API
    /// </summary>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task UploadVersionInformation(string system, string comp, string status);

    /// <summary>
    /// Get a Shields.io badge for the version of a component
    /// </summary>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <returns></returns>
    Task<ApiResponse<ShieldsIo>> GetInformation(string system, string comp);
}

/// <summary>
/// Service class for uploading and getting version information
/// Add this in program.cs: services.AddScoped;
/// </summary>
public class VersionService : IVersionService
{
    readonly string version;
    private readonly IVersionApi versionApi;
    private readonly ILogger<VersionService> logger;

    /// <summary>
    /// Service class for uploading and getting version information
    /// Add this in program.cs: services.AddScoped;
    /// </summary>
    /// <param name="versionApi"></param>
    /// <param name="logger"></param>
    public VersionService(IVersionApi versionApi, ILogger<VersionService> logger)
    {
        this.versionApi = versionApi;
        this.logger = logger;
        var assembly = Assembly.GetEntryAssembly()!;
        var fileversioninfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        var results = fileversioninfo.ProductVersion.Split('+');
        version=results[0];
    }

    /// <summary>
    /// Service method for uploading version information
    /// </summary>
    /// <param name="env"></param>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <param name="status"></param>
    public async Task UploadVersionInformation(string system, string comp, string status)
    {
        try
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            await versionApi.SetInformation(environment??"", system, comp, version, status);
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Error uploading version information");
        }
        
    }
    
    /// <summary>
    /// Service method for getting version information in a shields.io structure
    /// </summary>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <returns></returns>
    public async Task<ApiResponse<ShieldsIo>> GetInformation(string system, string comp)
    {
        try
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return await versionApi.GetInformation(environment??"", system, comp);
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Error getting version information");
            throw;
        }
    }   
}