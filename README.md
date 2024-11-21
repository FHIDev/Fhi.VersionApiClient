# Fhi.VersionApiClient

Small client to be used in clients that needs to upload version information to the Fhi.VersionApi.
This applies to components that can not be reached directly from the Fhi Azure DevOps service.

Use of this client allows the component to instead upload its version information to the Fhi Azure VersionApi service.

## Setup (using No authentication)

In program.cs, add the following:

```csharp
   services.AddVersionApiClient();
```

If you're using the Fhi.HelseId with Refit, add a section in the appsettings.json for HelseIdWebKonfigurasjon or HelseIdApiKonfigurasjon under the `Apis` section:

```json
   {
     "Name": "VersionService",
     "Url": "https://versionapi-felles-fhi.azurewebsites.net/"
   }
```

This url can be used for both test and production.

In the program.cs locate the `builder.AddHelseidRefitBuilder()`
and add the following line:

```csharp
   .AddRefitClient<IVersionApi>(nameof(VersionService))
```

## Usage

Create a service class that set up the system and component names, and retrieves the environment.

An example implementation can be like:

```csharp
namespace Fhi.Grunndata.OppslagWeb.Services;

public interface IOppslagsWebVersionService
{
    Task UploadVersionInfo(string status="healthy");
}

public class OppslagsWebVersionService(IVersionService versionService) : IOppslagsWebVersionService
{
    const string System = "Fhi.Grunndata";
    const string Component = "OppslagWeb";
    public async Task UploadVersionInfo(string status)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (!string.IsNullOrEmpty(environment))
            await versionService.UploadVersionInformation(environment, System, Component, status);
    }
}
```

Locate a suitable place in the component which is accessed at start, or regularly, like a Ping function, inject your implementation based onm OppslagsWebVersionService, and call the `UploadVersionInformation` method


