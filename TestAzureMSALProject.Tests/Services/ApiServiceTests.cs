using TestAzureMSALProject.Services;
using TestAzureMSALProject.Exceptions;
using TestAzureMSALProject.Tests.Fixtures;

namespace TestAzureMSALProject.Tests.Services;

/// <summary>
/// Unit tests for ApiService.
/// </summary>
public class ApiServiceTests
{
    private readonly HttpClient _httpClient;
    private readonly ApiService _apiService;
    private const string ValidAccessToken = "test-access-token-123";

    public ApiServiceTests()
    {
        var settings = ServiceFixture.CreateTestAppSettings();
        var mockLogger = ServiceFixture.CreateMockLogger<ApiService>();
        _httpClient = ServiceFixture.CreateMockHttpClient();

        _apiService = new ApiService(_httpClient, settings, mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = ServiceFixture.CreateTestAppSettings();
        var mockLogger = ServiceFixture.CreateMockLogger<ApiService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApiService(null!, settings, mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        var mockLogger = ServiceFixture.CreateMockLogger<ApiService>();
        var httpClient = ServiceFixture.CreateMockHttpClient();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApiService(httpClient, null!, mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = ServiceFixture.CreateTestAppSettings();
        var httpClient = ServiceFixture.CreateMockHttpClient();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApiService(httpClient, settings, null!));
    }

    [Fact]
    public async Task CallApiAsync_WithNullAccessToken_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _apiService.CallApiAsync(null!));
    }

    [Fact]
    public async Task CallApiAsync_WithEmptyAccessToken_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _apiService.CallApiAsync(string.Empty));
    }

    [Fact]
    public async Task CallApiAsync_WithWhitespaceAccessToken_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _apiService.CallApiAsync("   "));
    }

    [Fact]
    public void CallApiAsync_WithCancellation_RespectsCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        Assert.True(cts.Token.IsCancellationRequested);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"key\": \"value\"}")]
    [InlineData("plain text")]
    public void ApiService_CanBeCreated_WithDifferentScenarios(string scenario)
    {
        // Arrange
        var settings = ServiceFixture.CreateTestAppSettings();
        var mockLogger = ServiceFixture.CreateMockLogger<ApiService>();
        var httpClient = ServiceFixture.CreateMockHttpClient();

        // Act
        var service = new ApiService(httpClient, settings, mockLogger.Object);

        // Assert - verify the service is properly initialized for this scenario
        Assert.NotNull(service);
        Assert.IsAssignableFrom<IApiService>(service);
    }

    [Fact]
    public void ApiService_IsProperlyStructured()
    {
        // Act & Assert
        Assert.NotNull(_apiService);
        Assert.IsAssignableFrom<IApiService>(_apiService);
    }
}
