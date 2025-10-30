namespace TestAzureMSALProject.Services;

/// <summary>
/// Defines operations for Azure AD authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Acquires an access token for the configured scopes.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
