using System.Reflection;

using Microsoft.Extensions.Logging;

using Refit;

namespace Fhi.Grunndata.VersionApiClient;

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
    /// <param name="env"></param>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task UploadVersionInformation(string env, string system, string comp, string status);
    
    /// <summary>
    /// Get a Shields.io badge for the version of a component
    /// </summary>
    /// <param name="env"></param>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <returns></returns>
    Task<ApiResponse<ShieldsIo>> GetInformation(string env, string system, string comp);
}

/// <summary>
/// Service class for uploading and getting version information
/// Add this in program.cs: services.AddScoped;
/// </summary>
/// <param name="versionApi"></param>
/// <param name="logger"></param>
public class VersionService(IVersionApi versionApi, ILogger<VersionService> logger) : IVersionService
{
    readonly string version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "";
        
    /// <summary>
    /// Service method for uploading version information
    /// </summary>
    /// <param name="env"></param>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <param name="status"></param>
    public async Task UploadVersionInformation(string env, string system, string comp, string status)
    {
        try
        {
            await versionApi.SetInformation(env, system, comp, version, status);
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Error uploading version information");
        }
        
    }
    
    /// <summary>
    /// Service method for getting version information in a shields.io structure
    /// </summary>
    /// <param name="env"></param>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <returns></returns>
    public async Task<ApiResponse<ShieldsIo>> GetInformation(string env, string system, string comp)
    {
        try
        {
            return await versionApi.GetInformation(env, system, comp);
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Error getting version information");
            throw;
        }
    }   
}