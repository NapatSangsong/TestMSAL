using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using TestAzureMSALProject;
using TestAzureMSALProject.Services;
using TestAzureMSALProject.Exceptions;
using TestAzureMSALProject.Tests.Fixtures;

namespace TestAzureMSALProject.Tests;

/// <summary>
/// Unit tests for Application class.
/// </summary>
public class ApplicationTests
{
    [Fact]
    public void Constructor_WithNullAuthenticationService_ThrowsArgumentNullException()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var mockLogger = ServiceFixture.CreateMockLogger<Application>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Application(null!, mockApiService.Object, mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullApiService_ThrowsArgumentNullException()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockLogger = ServiceFixture.CreateMockLogger<Application>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Application(mockAuthService.Object, null!, mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockApiService = new Mock<IApiService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Application(mockAuthService.Object, mockApiService.Object, null!));
    }

    [Fact]
    public async Task RunAsync_WithValidServices_CompletesSuccessfully()
    {
        // Arrange
        const string testToken = "test-token";
        const string testResponse = "{\"result\": \"pong\"}";

        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testToken);

        var mockApiService = new Mock<IApiService>();
        mockApiService
            .Setup(x => x.CallApiAsync(testToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testResponse);

        var mockLogger = ServiceFixture.CreateMockLogger<Application>();

        var application = new Application(
            mockAuthService.Object,
            mockApiService.Object,
            mockLogger.Object);

        // Act
        await application.RunAsync();

        // Assert
        mockAuthService.Verify(
            x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        mockApiService.Verify(
            x => x.CallApiAsync(testToken, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenAuthenticationFails_ThrowsAuthenticationException()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthenticationException("Auth failed"));

        var mockApiService = new Mock<IApiService>();
        var mockLogger = ServiceFixture.CreateMockLogger<Application>();

        var application = new Application(
            mockAuthService.Object,
            mockApiService.Object,
            mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => application.RunAsync());

        // Verify that API call was never attempted
        mockApiService.Verify(
            x => x.CallApiAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RunAsync_WhenApiCallFails_ThrowsApiException()
    {
        // Arrange
        const string testToken = "test-token";

        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testToken);

        var mockApiService = new Mock<IApiService>();
        mockApiService
            .Setup(x => x.CallApiAsync(testToken, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiException("API call failed"));

        var mockLogger = ServiceFixture.CreateMockLogger<Application>();

        var application = new Application(
            mockAuthService.Object,
            mockApiService.Object,
            mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(() => application.RunAsync());

        // Verify that authentication was called but API failed
        mockAuthService.Verify(
            x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        mockApiService.Verify(
            x => x.CallApiAsync(testToken, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RunAsync_WithCancellationToken_CancelsGracefully()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockApiService = new Mock<IApiService>();
        var mockLogger = ServiceFixture.CreateMockLogger<Application>();

        var application = new Application(
            mockAuthService.Object,
            mockApiService.Object,
            mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            application.RunAsync(cts.Token));
    }

    [Fact]
    public async Task RunAsync_VerifiesWorkflowSequence()
    {
        // Arrange
        const string testToken = "test-token";
        const string testResponse = "response";
        var callSequence = new List<string>();

        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callSequence.Add("authenticate"))
            .ReturnsAsync(testToken);

        var mockApiService = new Mock<IApiService>();
        mockApiService
            .Setup(x => x.CallApiAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback(() => callSequence.Add("callApi"))
            .ReturnsAsync(testResponse);

        var mockLogger = ServiceFixture.CreateMockLogger<Application>();

        var application = new Application(
            mockAuthService.Object,
            mockApiService.Object,
            mockLogger.Object);

        // Act
        await application.RunAsync();

        // Assert
        Assert.Equal(new[] { "authenticate", "callApi" }, callSequence);
    }
}
