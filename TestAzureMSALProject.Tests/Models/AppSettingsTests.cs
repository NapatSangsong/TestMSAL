using Xunit;
using TestAzureMSALProject.Models;

namespace TestAzureMSALProject.Tests.Models;

/// <summary>
/// Unit tests for AppSettings model.
/// </summary>
public class AppSettingsTests
{
    [Fact]
    public void AppSettings_CanBeCreated_WithValidProperties()
    {
        // Arrange
        const string tenantId = "test-tenant";
        const string clientId = "test-client";
        const string apiUrl = "https://api.example.com";
        var scopes = new[] { "scope1", "scope2" };
        const string redirectUri = "http://localhost:3000";

        // Act
        var settings = new AppSettings
        {
            TenantId = tenantId,
            ClientId = clientId,
            ApiUrl = apiUrl,
            Scopes = scopes,
            RedirectUri = redirectUri
        };

        // Assert
        Assert.Equal(tenantId, settings.TenantId);
        Assert.Equal(clientId, settings.ClientId);
        Assert.Equal(apiUrl, settings.ApiUrl);
        Assert.Equal(scopes, settings.Scopes);
        Assert.Equal(redirectUri, settings.RedirectUri);
    }

    [Fact]
    public void AppSettings_DefaultScopes_IsEmpty()
    {
        // Arrange & Act
        var settings = new AppSettings
        {
            TenantId = "test",
            ClientId = "test",
            ApiUrl = "https://test.com",
            RedirectUri = "http://localhost"
        };

        // Assert
        Assert.Empty(settings.Scopes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("http://localhost:3000")]
    [InlineData("https://oauth.example.com/callback")]
    public void AppSettings_RedirectUri_CanHaveVariousValues(string? redirectUri)
    {
        // Arrange & Act
        var settings = new AppSettings
        {
            TenantId = "test",
            ClientId = "test",
            ApiUrl = "https://api.example.com",
            RedirectUri = redirectUri ?? "default"
        };

        // Assert
        Assert.NotNull(settings.RedirectUri);
    }

    [Fact]
    public void AppSettings_Scopes_CanBeModified()
    {
        // Arrange
        var settings = new AppSettings
        {
            TenantId = "test",
            ClientId = "test",
            ApiUrl = "https://api.example.com",
            RedirectUri = "http://localhost",
            Scopes = ["scope1"]
        };

        // Act
        settings.Scopes = ["scope1", "scope2", "scope3"];

        // Assert
        Assert.Equal(3, settings.Scopes.Length);
    }
}
