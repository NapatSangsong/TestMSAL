using Xunit;
using TestAzureMSALProject.Exceptions;

namespace TestAzureMSALProject.Tests.Exceptions;

/// <summary>
/// Unit tests for custom exceptions.
/// </summary>
public class ExceptionTests
{
    [Fact]
    public void ApiException_CanBeCreated_WithMessage()
    {
        // Arrange
        const string message = "API call failed";

        // Act
        var exception = new ApiException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void ApiException_CanBeCreated_WithMessageAndInnerException()
    {
        // Arrange
        const string message = "API call failed";
        var innerException = new HttpRequestException("Connection timeout");

        // Act
        var exception = new ApiException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void ApiException_IsException()
    {
        // Arrange
        var exception = new ApiException("Test");

        // Act & Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void AuthenticationException_CanBeCreated_WithMessage()
    {
        // Arrange
        const string message = "Authentication failed";

        // Act
        var exception = new AuthenticationException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void AuthenticationException_CanBeCreated_WithMessageAndInnerException()
    {
        // Arrange
        const string message = "Authentication failed";
        var innerException = new InvalidOperationException("Token expired");

        // Act
        var exception = new AuthenticationException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void AuthenticationException_IsException()
    {
        // Arrange
        var exception = new AuthenticationException("Test");

        // Act & Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void ApiException_PreservesMessage()
    {
        // Arrange
        const string message = "Original error";

        // Act
        var exception = new ApiException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void AuthenticationException_PreservesMessage()
    {
        // Arrange
        const string message = "Original auth error";

        // Act
        var exception = new AuthenticationException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Exception_WithNestedInnerException_PreservesStackTrace()
    {
        // Arrange
        var level3Exception = new Exception("Level 3");
        var level2Exception = new ApiException("Level 2", level3Exception);
        var level1Exception = new ApiException("Level 1", level2Exception);

        // Act & Assert
        Assert.Equal("Level 1", level1Exception.Message);
        Assert.Equal("Level 2", level1Exception.InnerException?.Message);
        Assert.Equal("Level 3", level1Exception.InnerException?.InnerException?.Message);
    }
}
