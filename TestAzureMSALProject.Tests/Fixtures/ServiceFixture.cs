using Microsoft.Extensions.Logging;
using Moq;
using TestAzureMSALProject.Models;

namespace TestAzureMSALProject.Tests.Fixtures;

/// <summary>
/// Provides common test fixtures and mocks for unit tests.
/// </summary>
public class ServiceFixture
{
    /// <summary>
    /// Creates a mock logger for testing.
    /// </summary>
    /// <typeparam name="T">The type to log for.</typeparam>
    /// <returns>A mock logger instance.</returns>
    public static Mock<ILogger<T>> CreateMockLogger<T>() where T : class
    {
        return new Mock<ILogger<T>>();
    }

    /// <summary>
    /// Creates default app settings for testing.
    /// </summary>
    /// <returns>AppSettings instance with test values.</returns>
    public static AppSettings CreateTestAppSettings()
    {
        return new AppSettings
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ApiUrl = "https://api.example.com/test",
            Scopes = ["api://test-scope"],
            RedirectUri = "http://localhost:3000"
        };
    }

    /// <summary>
    /// Creates a mock HttpClient for testing.
    /// </summary>
    /// <returns>A mock HttpClient instance.</returns>
    public static HttpClient CreateMockHttpClient()
    {
        return new HttpClient();
    }
}
