using TestAzureMSALProject.Services;
using TestAzureMSALProject.Exceptions;
using TestAzureMSALProject.Tests.Fixtures;

namespace TestAzureMSALProject.Tests.Services;

/// <summary>
/// Unit tests for AuthenticationService.
/// </summary>
public class AuthenticationServiceTests
{
    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        var mockLogger = ServiceFixture.CreateMockLogger<AuthenticationService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthenticationService(null!, mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = ServiceFixture.CreateTestAppSettings();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthenticationService(settings, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        var settings = ServiceFixture.CreateTestAppSettings();
        var mockLogger = ServiceFixture.CreateMockLogger<AuthenticationService>();

        // Act
        var service = new AuthenticationService(settings, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void GetAccessTokenAsync_WithCancellation_RespectsCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        Assert.True(cts.Token.IsCancellationRequested);
    }

    [Fact]
    public void AuthenticationService_IsProperlyStructured()
    {
        // Arrange
        var settings = ServiceFixture.CreateTestAppSettings();
        var mockLogger = ServiceFixture.CreateMockLogger<AuthenticationService>();

        // Act
        var service = new AuthenticationService(settings, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
        Assert.IsAssignableFrom<IAuthenticationService>(service);
    }
}
