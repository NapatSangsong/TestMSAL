namespace TestAzureMSALProject.Services;

/// <summary>
/// Defines operations for making authenticated API calls.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Calls the API endpoint with the provided access token.
    /// </summary>
    /// <param name="accessToken">The Bearer token for authentication.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The API response content.</returns>
    /// <exception cref="ApiException">Thrown when the API call fails.</exception>
    Task<string> CallApiAsync(string accessToken, CancellationToken cancellationToken = default);
}
