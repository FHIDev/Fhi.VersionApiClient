using System.Reflection;

using Microsoft.Extensions.Logging;

using Refit;

namespace Fhi.Grunndata.VersionApiClient;

public interface IVersionApi
{
    [Get("/api/SetInformation")]
    public Task SetInformation([Query] string environment, [Query] string system, [Query] string component, [Query] string version, [Query] string status);

    [Get("/api/GetInformation")]
    public Task<ApiResponse<ShieldsIo>> GetInformation([Query] string environment, [Query] string system, [Query] string component);
}

public interface IVersionService
{
    Task UploadVersionInformation(string env, string system, string comp, string status);
    Task<ApiResponse<ShieldsIo>> GetInformation(string env, string system, string comp);
}

public class VersionService(IVersionApi versionApi, ILogger<VersionService> logger) : IVersionService
{
    readonly string version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "";
        
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