﻿using System.Diagnostics;
using System.Reflection;
using Fhi.VersionApiClient.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;

namespace Fhi.VersionApiClient;

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
    public Task<string> SetInformation([Query] string environment, [Query] string system, [Query] string component, [Query] string version, [Query] string status);

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
    Task<string> UploadVersionInformation(string system, string comp, string status);

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
    private readonly IHostEnvironment hostEnvironment;
    private readonly ILogger<VersionService> logger;

    /// <summary>
    /// Service class for uploading and getting version information
    /// Add this in program.cs: services.AddScoped;
    /// </summary>
    /// <param name="versionApi"></param>
    /// <param name="hostEnvironment"></param>
    /// <param name="logger"></param>
    public VersionService(IVersionApi versionApi, IHostEnvironment hostEnvironment, ILogger<VersionService> logger)
    {
        this.hostEnvironment = hostEnvironment;
        this.versionApi = versionApi;
        this.logger = logger;
        var assembly = Assembly.GetEntryAssembly()!;
        var fileversioninfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        if (fileversioninfo.ProductVersion is null)
        {
            throw new ProductVersionMissingException();
        }

        var results = fileversioninfo.ProductVersion.Split('+');
        version = results[0];
    }

    /// <summary>
    /// Service method for uploading version information
    /// </summary>
    /// <param name="system"></param>
    /// <param name="comp"></param>
    /// <param name="status"></param>
    public async Task<string> UploadVersionInformation(string system, string comp, string status)
    {
        try
        {
            var environment = hostEnvironment.EnvironmentName;
            return await versionApi.SetInformation(environment, system, comp, version, status);
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Error uploading version information");
            return e.Message;
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
            var environment = hostEnvironment.EnvironmentName;
            return await versionApi.GetInformation(environment ?? "", system, comp);
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Error getting version information");
            throw;
        }
    }
}