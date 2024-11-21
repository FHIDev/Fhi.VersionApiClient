namespace Fhi.VersionApiClient;

/// <summary>
/// Configuration for the version client service.
/// </summary>
public class VersionApiConfiguration
{
    /// <summary>
    /// Url to the hosted (azure) version api
    /// </summary>
    public string VersionApiUrl { get; set; } = "";

    /// <summary>
    /// The name of the system.
    /// </summary>
    public string SystemName { get; set; } = "";

    /// <summary>
    /// The name of the component.
    /// </summary>
    public string ComponentName { get; set; } = "";
}