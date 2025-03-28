using Refit;

namespace Fhi.VersionApiClient.IntegrationTests;

[TestFixture]
public class VersionApiIntegrationTests
{
    private IVersionApi _versionApi = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _versionApi = RestService.For<IVersionApi>("https://versionapi-felles-fhi.azurewebsites.net");
    }

    [Test]
    public async Task GetInformation_ReturnsSuccess()
    {
        var response = await _versionApi.GetInformation("Development", "Fhi.Grunndata", "OppslagWeb");

        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(response.Content?.Label, Is.EqualTo("version").IgnoreCase);
    }

    [Test]
    public async Task SetInformation_ShouldSucceedOrAlreadyExist()
    {
        var response = await _versionApi.SetInformation(
            environment: "Development",
            system: "Fhi.Grunndata",
            component: "OppslagWeb",
            version: "1.2.3",
            status: "Green");

        Assert.That(response, Does.Contain("OK").Or.Contain("Updated existing").IgnoreCase);
    }
}